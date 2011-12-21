using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetflixWrapper.OAuth
{
    public class OAuthConfig
    {
        protected internal string OAuthVersion = "1.0";
        protected internal string OAuthParameterPrefix = "oauth_";
        protected internal string OAuthSignatureMethod = "HMAC-SHA1";

        //
        // List of know and used oauth parameters' names
        //        
        protected internal string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected internal string OAuthCallbackKey = "oauth_callback";
        protected internal string OAuthVersionKey = "oauth_version";
        protected internal string OAuthSignatureMethodKey = "oauth_signature_method";
        protected internal string OAuthSignatureKey = "oauth_signature";
        protected internal string OAuthTimeStampKey = "oauth_timestamp";
        protected internal string OAuthNonceKey = "oauth_nonce";
        protected internal string OAuthTokenKey = "oauth_token";
        protected internal string OAuthTokenSecretKey = "oauth_token_secret";

        public Uri RequestTokenUri { get; set; }
        public Uri AccessTokenUri { get; set; }

        public string OAuthConsumerKey { get; set; }
        public string OAuthConsumerSecret { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }

        protected internal bool IsValidForRequestToken()
        {
            if (RequestTokenUri == null)
                throw new ArgumentNullException("RequestTokenUrl");
            if (string.IsNullOrWhiteSpace(OAuthConsumerKey))
                throw new ArgumentNullException("ConsumerKey");
            if (string.IsNullOrWhiteSpace(OAuthConsumerSecret))
                throw new ArgumentNullException("ConsumerSecret");
            return true;
        }

        protected internal bool IsValidForAccessToken()
        {
            if (AccessTokenUri == null)
                throw new ArgumentNullException("AccessTokenUrl");
            if (string.IsNullOrWhiteSpace(OAuthConsumerKey))
                throw new ArgumentNullException("ConsumerKey");
            if (string.IsNullOrWhiteSpace(OAuthConsumerSecret))
                throw new ArgumentNullException("ConsumerSecret");
            if (string.IsNullOrWhiteSpace(OAuthToken))
                throw new ArgumentNullException("OAuthToken");
            if (string.IsNullOrWhiteSpace(OAuthTokenSecret))
                throw new ArgumentNullException("OAuthTokenSecret");
            return true;
        }
    }
}
