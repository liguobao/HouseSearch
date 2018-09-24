using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.Common;
using HouseMapAPI.CommonException;
using HouseMapAPI.Dapper;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class UserService
    {

        private RedisService _redisService;

        private UserDapper _userDapper;

        private EmailService _emailService;

        private IOAuthClient _authClient;

        private WeChatAppDecrypt _weChatAppDecrypt;

        public UserService(RedisService redisService, UserDapper userDapper,
         EmailService emailService, QQOAuthClient authClient,
          WeChatAppDecrypt weChatAppDecrypt)
        {

            _redisService = redisService;
            _userDapper = userDapper;
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
            var userInfo = _userDapper.FindUser(insertUser.UserName);
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
            var userInfo = _userDapper.FindUser(loginUser.UserName);
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
            var userInfo = _userDapper.FindByQQOpenID(qqUser.Id);
            if (userInfo == null)
            {
                //新增用户
                _userDapper.InsertUserForQQAuth(new UserInfo()
                {
                    UserName = qqUser.Name,
                    QQOpenUID = qqUser.Id,
                    AvatarUrl = qqUser.ImgUrl,
                    JsonData = JsonConvert.SerializeObject(qqUser)
                });
                userInfo = _userDapper.FindByQQOpenID(qqUser.Id);
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
            insertUser.Password = registerUser.Password;
            insertUser.ActivatedCode = activatecode;
            _userDapper.InsertUser(insertUser);
            return insertUser;
        }

        private void SendActivateEmail(UserSave registerUser, string token)
        {
            EmailInfo email = new EmailInfo();
            email.Body = $"Hi,{registerUser.UserName}. <br>欢迎您注册地图搜租房(woyaozufang.live),你的账号已经注册成功." +
            "<br/>为了保证您能正常体验网站服务，请点击下面的链接完成邮箱验证以激活账号."
            + $"<br><a href='https://woyaozufang.live/Account/Activated?activatedCode={token}'>https://woyaozufang.live/Account/Activate?activatedCode={token}</a> "
            + "<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
            + "<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。" +
            "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
            email.Receiver = registerUser.Email;
            email.Subject = "地图找租房-激活账号";
            email.ReceiverName = registerUser.UserName;
            _emailService.Send(email);
        }

        private void CheckRegisterUser(UserSave registerUser)
        {
            if (registerUser == null || string.IsNullOrEmpty(registerUser.Email) || string.IsNullOrEmpty(registerUser.UserName))
            {
                throw new Exception("用户名/用户邮箱不能为空.");
            }
            var checkUser = _userDapper.FindUser(registerUser.UserName);
            if (checkUser != null)
            {
                throw new Exception("用户已存在!");
            }
        }

        public UserInfo GetUserInfo(long userId, string token)
        {
            var userToken = _redisService.ReadCache(RedisKey.UserToken.Key + userId, RedisKey.UserToken.DBName);
            if (string.IsNullOrEmpty(userToken))
            {
                throw new TokenInvalidException("can not find token in redis");
            }

            if (userToken != token)
            {
                throw new TokenInvalidException("token invalid");
            }
            return _redisService.ReadCache<UserInfo>(RedisKey.UserId.Key + userId, RedisKey.UserId.DBName);
        }

        public UserInfo GetUserByToken(string token)
        {
            var userJson = _redisService.ReadCache(token, 0);
            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonConvert.DeserializeObject<UserInfo>(userJson);
            }
            return null;
        }


        public void WriteUserToken(UserInfo loginUser, string token)
        {
            _redisService.WriteObject(RedisKey.UserToken.Key + loginUser.ID, token, 0, 60 * 24 * 30);
            _redisService.WriteObject(RedisKey.UserId.Key + loginUser.ID, loginUser, 0, 60 * 24 * 30);
            _redisService.WriteObject(token, loginUser, 0, 60 * 24 * 30);
        }

        public UserInfo FindUser(string userName)
        {
            return _userDapper.FindUser(userName);
        }

        public bool SaveWorkAddress(long userID, string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new UnProcessableException("地址不能为空!");
            }
            var success = _userDapper.SaveWorkAddress(userID, address);
            RefreshUserCache(userID);
            return success;
        }

        private void RefreshUserCache(long userID)
        {
            var token = _redisService.ReadCache<string>(RedisKey.UserToken.Key + userID, RedisKey.UserToken.DBName);
            var user = _userDapper.FindByID(userID);
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
            var userInfo = _userDapper.FindByWechatOpenID(wechatUser.openId);
            if (userInfo == null)
            {
                _userDapper.InsertUserForWechat(new UserInfo()
                {
                    UserName = wechatUser.nickName,
                    WechatOpenID = wechatUser.openId,
                    AvatarUrl = wechatUser.avatarUrl,
                    JsonData = JsonConvert.SerializeObject(wechatUser)
                });
            }
            string token = userInfo.NewLoginToken;
            WriteUserToken(userInfo, token);
            return Tuple.Create<string, UserInfo>(token, userInfo);
        }



    }
}