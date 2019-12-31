using System;
using System.Linq;
using HouseCrawler.Core.Service;
using Microsoft.Extensions.Options;
using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class TodayHouseDashboardJob : Job
    {
        EmailService emailService;

        APPConfiguration configuration;

        HouseDapper houseDapper;

        public TodayHouseDashboardJob(EmailService emailService, IOptions<APPConfiguration> configuration,
         HouseDapper houseDapper)
        {
            this.emailService = emailService;
            this.configuration = configuration.Value;
            this.houseDapper = houseDapper;
        }

        [Invoke(Begin = "2018-07-01 23:00", Interval = 1000 * 3600 *8, SkipWhileExecuting = true)]
        public void Run()
        {
            var today = DateTime.Now.ToLocalTime();
            EmailInfo email = new EmailInfo();
            email.Receiver = configuration.ReceiverAddress;
            email.ReceiverName = configuration.ReceiverName;
            email.Subject = $"地图搜租房每日数据汇总({today.ToString("yyyy-MM-dd")})";
            var statList = houseDapper.GetHouseStatList();
            string bodyHTML = @"<table border='1' cellpadding='0' cellspacing='0' width='100%'> 
             <tr> 
             <td>来源</td>
             <td>数量</td>
             <td>最晚发布时间</td>
             <td>最晚入库时间</td> 
             </tr>";
            foreach (var stat in statList)
            {
                bodyHTML = bodyHTML + $" <tr> <td>{stat.Source}</td><td>{stat.HouseSum}</td><td>{stat.LastPubTime}</td> <td>{stat.LastCreateTime}</td> </tr>";
            }
            bodyHTML = bodyHTML + $" <tr> <td>共计</td><td>{statList.Sum(s => s.HouseSum)}</td> </tr>";
            bodyHTML = bodyHTML + " </table>";
            email.Body = bodyHTML;
            emailService.Send(email);
            
        }
    }
}
