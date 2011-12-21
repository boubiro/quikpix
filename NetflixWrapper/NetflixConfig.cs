using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetflixWrapper
{
    public class NetflixConfig
    {
        public const string APPLICATION_NAME = "QuikPix for Netflix";

        public const string REQUEST_TOKEN_URL = "http://api.netflix.com/oauth/request_token";
        public const string ACCESS_TOKEN_URL = "http://api.netflix.com/oauth/access_token";
        public const string USER_AUTHORIZATION_URL = "https://api-user.netflix.com/oauth/login";

        public const string CONSUMER_KEY = "nvcgk2wk6zsukejaeknbs7ay";
        public const string CONSUMER_SECRET = "b2NzjJDHmA";

        public string UserID { get; set; }
        public string UserToken { get; set; }
        public string UserSecret { get; set; }

        public string LoginUrlBase { get; set; }

    }
}
