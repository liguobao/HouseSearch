using System;
using Newtonsoft.Json;

namespace HouseMapAPI.Models
{

    public class UserSave
    {
        public long ID { get; set; }

        public string UserName { get; set; }



        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime DataCreateTime { get; set; }

        public DateTime DataChange_LastTime { get; set; }


        [JsonIgnore]
        public string RetrievePasswordToken { get; set; }
        
        ///0:未激活 1:已激活 2:已禁用
        public int Status { get; set; }

        public string Mobile { get; set; }

        public string QQ { get; set; }

        public string Intro { get; set; }

        public string QQOpenUID { get; set; }

    }
}