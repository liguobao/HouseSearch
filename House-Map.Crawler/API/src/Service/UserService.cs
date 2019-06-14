using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.CommonException;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;
using HouseMapAPI.Models;
using System.Linq;

namespace HouseMapAPI.Service
{

    public class UserService
    {

        private readonly RedisTool _RedisTool;

        private readonly HouseMapContext _context;

        private EmailService _emailService;

        private IOAuthClient _authClient;

        private WeChatAppDecrypt _weChatAppDecrypt;

        public UserService(RedisTool RedisTool, HouseMapContext context,
         EmailService emailService, QQOAuthClient authClient,
          WeChatAppDecrypt weChatAppDecrypt)
        {

            _RedisTool = RedisTool;
            _context = context;
            _emailService = emailService;
            _authClient = authClient.GetAPIOAuthClient();
            _weChatAppDecrypt = weChatAppDecrypt;
        }

        public Tuple<string, UserInfo> Register(UserSave registerUser)
        {
            CheckRegisterUser(registerUser);
            string activatecode = Tools.GetSha256(registerUser.UserName + registerUser.Email + DateTime.Now);
            SendActivateEmail(registerUser, activatecode);
            UserInfo insertUser = AddUser(registerUser, activatecode);
            var userInfo = _context.Users.FirstOrDefault(u => u.UserName == registerUser.UserName);
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);

        }

        public Tuple<string, UserInfo> Activated(string code)
        {
            var userInfo = _context.Users.FirstOrDefault(u => u.ActivatedCode == code);
            if (userInfo == null)
            {
                throw new NotFoundException($"{code} invalid,user not found.");
            }
            userInfo.Status = 1;
            _context.SaveChanges();
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);
        }

        public Tuple<string, UserInfo> Login(UserSave loginUser)
        {
            if (loginUser == null || string.IsNullOrEmpty(loginUser.UserName))
            {
                throw new Exception("用户名/用户邮箱不能为空.");
            }
            var userInfo = _context.Users.FirstOrDefault(u => u.UserName == loginUser.UserName || u.Email == loginUser.UserName);
            CheckLogin(loginUser, userInfo);
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);
        }


        public Tuple<string, UserInfo> OAuthCallback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("无效的auth code");
            }

            var accessToken = _authClient.GetAccessToken(code).Result;
            var qqUser = _authClient.GetUserInfo(accessToken).Result;
            //未登录,通过此ID获取用户
            var userInfo = _context.Users.FirstOrDefault(u => u.QQOpenUID == qqUser.Id);
            if (userInfo == null)
            {
                //新增用户
                var user = new UserInfo()
                {
                    UserName = qqUser.Name,
                    QQOpenUID = qqUser.Id,
                    AvatarUrl = qqUser.ImgUrl,
                    JsonData = JsonConvert.SerializeObject(qqUser)
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                userInfo = user;
            }
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);
        }


        private static void CheckLogin(UserSave loginUser, UserInfo userInfo)
        {
            if (userInfo == null)
            {
                throw new Exception("找不到用户信息或密码错误!");
            }
            if (userInfo.Password != Tools.GetMD5(loginUser.Password))
            {
                throw new Exception("用户名/密码错误.");
            }
        }

        private UserInfo AddUser(UserSave registerUser, string activatecode)
        {
            var insertUser = new UserInfo();
            insertUser.Email = registerUser.Email;
            insertUser.UserName = registerUser.UserName;
            insertUser.Password = Tools.GetMD5(registerUser.Password);
            insertUser.ActivatedCode = activatecode;
            _context.Users.Add(insertUser);
            _context.SaveChanges();
            return insertUser;
        }

        private void SendActivateEmail(UserSave registerUser, string token)
        {
            EmailInfo email = new EmailInfo();
            email.Body = $"Hi,{registerUser.UserName}. <br>欢迎注册地图搜租房(house-map.cn),您的账号已经注册成功." +
            "<br/>为了保证您能正常体验网站服务，请点击下面的链接完成邮箱验证以激活账号."
            + $"<br><a href='https://woyaozufang.live/#/Account/Activated?code={token}'>https://woyaozufang.live/#/Account/Activate?code={token}</a> "
            + "<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
            + "<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。" +
            "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
            email.Receiver = registerUser.Email;
            email.Subject = "地图搜租房-激活账号";
            email.ReceiverName = registerUser.UserName;
            _emailService.Send(email);
        }

        private void CheckRegisterUser(UserSave registerUser)
        {
            if (registerUser == null || string.IsNullOrEmpty(registerUser.Email) || string.IsNullOrEmpty(registerUser.UserName))
            {
                throw new Exception("用户名/用户邮箱不能为空.");
            }
            if (_context.Users.Any(u => u.UserName == registerUser.UserName))
            {
                throw new Exception("用户已存在!");
            }
        }

        public UserInfo GetUserInfo(long userId, string token)
        {
            var userToken = _RedisTool.ReadCache(RedisKeys.UserToken.Key + userId, RedisKeys.UserToken.DBName);
            if (string.IsNullOrEmpty(userToken))
            {
                throw new TokenInvalidException("can not find token in redis");
            }

            if (userToken != token)
            {
                throw new TokenInvalidException("token invalid");
            }
            return _RedisTool.ReadCache<UserInfo>(RedisKeys.UserId.Key + userId, RedisKeys.UserId.DBName);
        }

        public UserInfo GetUserByToken(string token)
        {
            return _RedisTool.ReadCache<UserInfo>(token, 0);
        }


        public void WriteUserToken(UserInfo loginUser, string token)
        {
            _RedisTool.WriteObject(RedisKeys.UserToken.Key + loginUser.ID, token, 0, 60 * 24 * 30);
            _RedisTool.WriteObject(RedisKeys.UserId.Key + loginUser.ID, loginUser, 0, 60 * 24 * 30);
            _RedisTool.WriteObject(token, loginUser, 0, 60 * 24 * 30);
        }



        public bool SaveWorkAddress(long userID, string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new UnProcessableException("地址不能为空!");
            }
            var user = _context.Users.FirstOrDefault(u => u.ID == userID);
            if (user == null)
            {
                throw new UnProcessableException("用户信息不存在,请重新登录!");
            }
            user.WorkAddress = address;
            _context.SaveChanges();
            RefreshUserCache(userID);
            return true;
        }


        public bool SaveEmail(long userID, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new UnProcessableException("地址不能为空!");
            }
            // 检查邮件是否已存在
            if (_context.Users.Any(u => u.Email == email && u.ID != userID))
            {
                throw new UnProcessableException("此邮箱已绑定其他账号,请尝试找回密码后重新登录!");
            }
            var user = _context.Users.FirstOrDefault(u => u.ID == userID);
            if (user == null)
            {
                throw new UnProcessableException("用户信息不存在,请重新登录!");
            }
            user.Email = email;
            _context.SaveChanges();
            RefreshUserCache(userID);
            return true;
        }

        private void RefreshUserCache(long userID)
        {
            var token = _RedisTool.ReadCache<string>(RedisKeys.UserToken.Key + userID, RedisKeys.UserToken.DBName);
            var user = _context.Users.FirstOrDefault(u => u.ID == userID);
            _RedisTool.DeleteCache(RedisKeys.UserId.Key + userID);
            _RedisTool.DeleteCache(token);
            WriteUserToken(user, token);
        }

        public Tuple<string, UserInfo> WechatLogin(WechatLoginInfo loginInfo)
        {
            Console.WriteLine($"WeixinLoginInfo:{JsonConvert.SerializeObject(loginInfo)}");
            var wechatUser = _weChatAppDecrypt.Decrypt(loginInfo);
            if (wechatUser == null)
            {
                throw new TokenInvalidException("解密微信用户信息失败.");
            }
            var userInfo = _context.Users.FirstOrDefault(u => u.WechatOpenID == wechatUser.openId);
            if (userInfo == null)
            {
                userInfo = new UserInfo()
                {
                    UserName = wechatUser.nickName,
                    WechatOpenID = wechatUser.openId,
                    AvatarUrl = wechatUser.avatarUrl,
                    JsonData = JsonConvert.SerializeObject(wechatUser)
                };
                _context.Users.Add(userInfo);
                _context.SaveChanges();
            }
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);
        }

    }
}