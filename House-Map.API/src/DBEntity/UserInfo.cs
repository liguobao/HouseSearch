using System;
using HouseMapAPI.Common;
using Newtonsoft.Json;

namespace HouseMapAPI.DBEntity
{

    public class UserInfo
    {
        public long ID { get; set; }

        public string UserName { get; set; }

        [JsonIgnore]
        public string ActivatedCode { get; set; }

        [JsonIgnore]
        public string ActivatedTime { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public DateTime DataCreateTime { get; set; } = DateTime.Now;

        [JsonIgnore]
        public DateTime DataChange_LastTime { get; set; } = DateTime.Now;


        [JsonIgnore]
        public string RetrievePasswordToken { get; set; }

        [JsonIgnore]
        public DateTime TokenTime { get; set; }
        ///0:未激活 1:已激活 2:已禁用
        public int Status { get; set; }

        public string Mobile { get; set; }

        public string QQ { get; set; }

        public string Intro { get; set; }

        [JsonIgnore]
        public string QQOpenUID { get; set; }


        [JsonIgnore]
        public string WechatOpenID { get; set; }

        public string AvatarUrl { get; set; }

        [JsonIgnore]
        public string JsonData { get; set; }

        [JsonIgnore]
        public string NewLoginToken
        {
            get
            {
                return Tools.GetMD5($"{this.ID}|{this.UserName}|{DateTime.Now.ToString() + this.DataCreateTime}");
            }
        }

        public string WorkAddress { get; set; }

    }
}