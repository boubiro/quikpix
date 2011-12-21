using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace NetflixWrapper.OAuth
{
    public class OAuthHandler
    {
        public OAuthConfig Config { get; protected set; }
        
        public OAuthHandler() : this(new OAuthConfig()) { }

        public OAuthHandler(OAuthConfig config)
        {
            if (config == null)
                throw new ArgumentException("Config is required.");
            this.Config = config;
        }

        public NameValueCollection GetRequestToken()
        {
            return GetRequestToken(null);
        }

        public NameValueCollection GetRequestToken(NameValueCollection requestParams)
        {
            if (this.Config.IsValidForRequestToken())
            {
                var request = OAuthRequest.GenerateSignedRequest(this.Config, this.Config.RequestTokenUri, requestParams, OAuthRequest.HttpMethods.GET);
                var response = request.SendRequest();

                var responseParams = HttpUtility.ParseQueryString(response);
                if (responseParams != null && responseParams.Count > 0)
                {
                    Config.OAuthToken = (responseParams[Config.OAuthTokenKey] ?? "");
                    responseParams.Remove(Config.OAuthTokenKey);
                    Config.OAuthTokenSecret = (responseParams[Config.OAuthTokenSecretKey] ?? "");
                    responseParams.Remove(Config.OAuthTokenSecretKey);

                    return responseParams;
                }
            }

            return null;
        }

        public NameValueCollection GetAccessToken()
        {
            if (this.Config.IsValidForAccessToken())
            {
                var request = OAuthRequest.GenerateProtectedRequest(this.Config, this.Config.AccessTokenUri, null, OAuthRequest.HttpMethods.GET);
                var response = request.SendRequest();

                var responseParams = HttpUtility.ParseQueryString(response);
                if (responseParams != null && responseParams.Count > 0)
                {
                    Config.OAuthToken = (responseParams[Config.OAuthTokenKey] ?? "");
                    responseParams.Remove(Config.OAuthTokenKey);
                    Config.OAuthTokenSecret = (responseParams[Config.OAuthTokenSecretKey] ?? "");
                    responseParams.Remove(Config.OAuthTokenSecretKey);

                    return responseParams;
                }
            }
            return null;
        }

        internal string SendConsumerRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return OAuthRequest.GenerateConsumerRequest(this.Config, requestUri, requestParams, httpMethod).SendRequest();
        }

        public string SendSignedRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return OAuthRequest.GenerateSignedRequest(this.Config, requestUri, requestParams, httpMethod).SendRequest();
        }

        public string SendProtectedRequest(Uri requestUri, NameValueCollection requestParams, OAuthRequest.HttpMethods httpMethod)
        {
            return OAuthRequest.GenerateProtectedRequest(this.Config, requestUri, requestParams, httpMethod).SendRequest();
        }
    }
}
