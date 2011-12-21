using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using NetflixWrapper.OAuth;
using Microsoft.Win32;

namespace NetflixWrapper
{
    public class NetflixHandler
    {
        public NetflixConfig Config { get; private set; }
        public NetflixUserHandler Users { get; private set; }
        public NetflixCatalogHandler Catalog { get; private set; }

        protected readonly OAuthHandler oauthHandler;

        public NetflixHandler()
            : this(null, null, null) { }

        public NetflixHandler(string userToken, string userSecret, string userID)
        {
            this.Config = new NetflixConfig()
            {
                UserToken = (userToken ?? ""),
                UserSecret = (userSecret ?? ""),
                UserID = (userID ?? "")
            };

            var oauthConfig = new OAuthConfig()
            {
                OAuthConsumerKey = NetflixConfig.CONSUMER_KEY,
                OAuthConsumerSecret = NetflixConfig.CONSUMER_SECRET,
                OAuthToken = this.Config.UserToken,
                OAuthTokenSecret = this.Config.UserSecret,

                RequestTokenUri = new Uri(NetflixConfig.REQUEST_TOKEN_URL),
                AccessTokenUri = new Uri(NetflixConfig.ACCESS_TOKEN_URL)
            };

            this.oauthHandler = new OAuthHandler(oauthConfig);

            this.Users = new NetflixUserHandler(this);
            this.Catalog = new NetflixCatalogHandler(this);
        }

        public void AuthorizeApplication()
        {
            if (string.IsNullOrWhiteSpace(oauthHandler.Config.OAuthToken) || string.IsNullOrWhiteSpace(oauthHandler.Config.OAuthTokenSecret))
            {
                GetRequestToken();
                ProcessLogin();
                GetAccessToken();
            }
        }


        private void GetRequestToken()
        {
            var response = this.oauthHandler.GetRequestToken();
            this.Config.LoginUrlBase = response["login_url"];
        }

        private void ProcessLogin()
        {
            var loginUrl = string.Format("{0}?&oauth_consumer_key={1}&application_name={2}", Config.LoginUrlBase, NetflixConfig.CONSUMER_KEY, NetflixConfig.APPLICATION_NAME);

            OpenUrl(loginUrl);
        }

        private void GetAccessToken()
        {
            var response = oauthHandler.GetAccessToken();
            this.Config.UserToken = oauthHandler.Config.OAuthToken;
            this.Config.UserSecret = oauthHandler.Config.OAuthTokenSecret;
            this.Config.UserID = response["user_id"];
        }

        protected internal string SendConsumerRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return oauthHandler.SendConsumerRequest(requestUri, requestParams, httpMethod);
        }

        protected internal string SendSignedRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return oauthHandler.SendSignedRequest(requestUri, requestParams, httpMethod);
        }

        protected internal string SendProtectedRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return oauthHandler.SendProtectedRequest(requestUri, requestParams, httpMethod);
        }

        private void OpenUrl(string url)
        {
            var proc = new Process();
            proc.StartInfo.FileName = GetDefaultBrowser();
            proc.StartInfo.Arguments = url;
            proc.Start();

            proc.WaitForInputIdle();

            proc.WaitForExit();
        }




        //http://dotnetpulse.blogspot.com/2006/04/opening-url-from-within-c-program.html
        private static string GetDefaultBrowser()
        {
            string key = @"htmlfile\shell\open\command";
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(key, false);
            // get default browser path
            return ((string)registryKey.GetValue(null, null)).Split('"')[1];
        }
    }
}
