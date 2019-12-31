using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using Talk.OAuthClient;

namespace HouseCrawler.Web.Service
{

    public class QQOAuthClient
    {
        APPConfiguration _configuration;

        public QQOAuthClient(IOptions<APPConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }


        public IOAuthClient GetOAuthClient()
        {
            return OAuthClientFactory.GetOAuthClient(_configuration.QQAPPID,
             _configuration.QQAPPKey, _configuration.QQAuthReturnURL, AuthType.QQ);
        }


        public IOAuthClient GetAPIOAuthClient()
        {
            return OAuthClientFactory.GetOAuthClient(_configuration.QQAPIID,
             _configuration.QQAPIKey, _configuration.QQAPIAuthReturnURL, AuthType.QQ);
        }
    }
}