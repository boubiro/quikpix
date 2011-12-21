using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetflixWrapper
{
    public class NetflixUserHandler
    {
        private readonly NetflixHandler handler;

        private string UserID { get { return handler.Config.UserID; } }
        private string UserBaseUrl { get { return "http://api.netflix.com/users/"; } }
        private string UserUrl { get { return string.Format("{0}{1}", UserBaseUrl, UserID); } }

        protected internal NetflixUserHandler(NetflixHandler handler)
        {
            this.handler = handler;
        }

        public void RequestCurrentUser()
        {
            var uri = new Uri(UserBaseUrl + "current");

            var response = handler.SendProtectedRequest(uri, null, OAuth.OAuthRequest.HttpMethods.GET);
        }

        public void RequestUserDetails()
        {
            var uri = new Uri(UserUrl);

            var response = handler.SendProtectedRequest(uri, null, OAuth.OAuthRequest.HttpMethods.GET);
        }

        public void RequestUserFeeds()
        {
            var uri = new Uri(UserUrl + "/feeds");
            var response = handler.SendProtectedRequest(uri, null, OAuth.OAuthRequest.HttpMethods.GET);
        }

        public void RequestAvailableQueues()
        {
            var uri = new Uri(UserUrl + "/queues/instant/available");
            var response = handler.SendProtectedRequest(uri, null, OAuth.OAuthRequest.HttpMethods.GET);
        }
        
    }
}
