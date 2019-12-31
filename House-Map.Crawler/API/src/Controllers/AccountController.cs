using Microsoft.AspNetCore.Mvc;
using HouseMapAPI.Service;
using Talk.OAuthClient;
using Microsoft.AspNetCore.Cors;
using HouseMapAPI.Models;

namespace HouseMapAPI.Controllers
{

    [Route("v1/[controller]")]
    public class AccountController : ControllerBase
    {

        private UserService _userService;

        private IOAuthClient _authClient;


        public AccountController(UserService userService,
                          QQOAuthClient authClient)
        {
            this._userService = userService;
            _authClient = authClient.GetAPIOAuthClient();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [EnableCors("APICors")]
        [HttpPost("register", Name = "Register")]
        public ActionResult Register([FromBody]UserSave registerUser)
        {
            var result = _userService.Register(registerUser);
            return Ok(new { success = true, message = "注册成功!", token = result.Item1, data = result.Item2 });

        }


        /// <summary>
        /// 用户登录
        /// </summary>
        [EnableCors("APICors")]
        [HttpPost("", Name = "Login")]
        public ActionResult Login([FromBody]UserSave loginUser)
        {

            var result = _userService.Login(loginUser);
            return Ok(new { success = true, message = "登录成功!", token = result.Item1, data = result.Item2 });
        }


        /// <summary>
        /// 第三方登录回调
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("callback", Name = "Callback")]
        public ActionResult Callback([FromQuery]string code, [FromQuery]string state)
        {
            var result = _userService.OAuthCallback(code);
            return Ok(new { success = true, token = result.Item1, message = "登录成功!", data = result.Item2 });
        }

        /// <summary>
        /// 激活用户
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("activated/{code}", Name = "Activated")]
        public ActionResult Activated(string code)
        {
            var result = _userService.Activated(code);
            return Ok(new { success = true, token = result.Item1, message = "激活成功!", data = result.Item2 });
        }


        /// <summary>
        /// 微信登录
        /// </summary>
        [EnableCors("APICors")]
        [HttpPost("weixin", Name = "Weixin")]
        public ActionResult Weixin([FromBody]WechatLoginInfo loginInfo)
        {
            var result = _userService.WechatLogin(loginInfo);
            return Ok(new { success = true, token = result.Item1, message = "登录成功!", data = result.Item2 });
        }


        /// <summary>
        /// 获取QQ登录URL
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("oauth-url", Name = "GetQQOAuthUrl")]
        public IActionResult GetQQOAuthUrl()
        {
            var url = _authClient.GetAuthUrl();
            return Ok(new { success = true, url = url });
        }

    }
}