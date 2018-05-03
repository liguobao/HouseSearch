using System;

public class UserInfo
{
    public long ID { get; set; }

    public string UserName { get; set; }

    public string ActivatedCode {get;set;}


    public string ActivatedTime{get;set;}


    public string Email{get;set;}

    public string Password { get; set; }

    public DateTime DataCreateTime { get; set; }

    public DateTime DataChange_LastTime { get; set; }
    ///0:未激活 1:已激活 2:已禁用
    public int Status;

}