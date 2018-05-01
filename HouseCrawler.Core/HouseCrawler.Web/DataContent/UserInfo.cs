using System;

public class UserInfo
{
    public long ID { get; set; }

    public string UserName { get; set; }


    public string UserEmail { get; set; }

    public string Token { get; set; }

    public DateTime TokenTimestamp { get; set; }

    public DateTime TokenLimitTime { get; set; }

    public DateTime DataCreateTime { get; set; }

    public DateTime DataChange_LastTime { get; set; }

}