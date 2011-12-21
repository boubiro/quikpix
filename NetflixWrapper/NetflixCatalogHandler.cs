using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetflixWrapper
{
    public class NetflixCatalogHandler
    {
        protected NetflixHandler Handler { get; private set; }

        public string BaseUrl { get { return "http://api.netflix.com/catalog"; } }

        protected internal NetflixCatalogHandler(NetflixHandler handler) {
            this.Handler = handler;
        }

        public void RequestCatalogTitles()
        {
            var uri = new Uri(BaseUrl + "/titles/full?v=2.0");

            var response = Handler.SendProtectedRequest(uri, null, OAuth.OAuthRequest.HttpMethods.GET);
        }


    }
}
