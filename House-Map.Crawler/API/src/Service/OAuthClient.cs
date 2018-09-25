using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Common;
using Microsoft.Extensions.Options;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class QQOAuthClient
    {
        AppSettings _configuration;

        public QQOAuthClient(IOptions<AppSettings> configuration)
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