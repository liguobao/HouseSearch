using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Common;
using Microsoft.Extensions.Options;

namespace HouseMapAPI.Service
{

    public class EmailService
    {
        AppSettings configuration;

        public EmailService(IOptions<AppSettings> configuration)
        {
            this.configuration = configuration.Value;
        }

        public void Send(EmailInfo emial)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Port = configuration.EmailSMTPPort;
                smtp.Host = configuration.EmailSMTPDomain;
                smtp.Timeout = 10000;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(configuration.EmailAccount, configuration.EmailPassword);
                smtp.Send(ConvertToMail(emial));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        private MailMessage ConvertToMail(EmailInfo emial)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(configuration.SenderAddress);
            mail.To.Add(new MailAddress(emial.Receiver, emial.ReceiverName));
            mail.Subject = emial.Subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = emial.Body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            return mail;
        }
    }


    public class EmailInfo
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }
    }

}

