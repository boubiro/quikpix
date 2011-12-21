using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetflixWrapper.OAuth;
using System.Diagnostics;
using NetflixWrapper.Netflix;

namespace NetflixWrapper
{
    public class Class1
    {

        public static void Main()
        {
            var c = new Class1();
            c.Test();
        }


        public void Test()
        {
            NetflixCatalog catalog = new NetflixCatalog(new Uri("http://odata.netflix.com/Catalog/"));


            var genres = catalog.Genres;
            foreach (var genre in genres)
            {
                var titles = (from g in catalog.Genres
                              from t in g.Titles
                              where g.Name == genre.Name
                              orderby t.AverageRating 
                              select t).Take(5).ToList();
                foreach (var title in titles)
                {
                    Console.WriteLine(title.Name);
                }
            }

            Console.ReadKey();

            /*
            var netflixHandler = new NetflixHandler();
            netflixHandler.AuthorizeApplication();
            */
            /*
            var netflixHandler = new NetflixHandler("BQAJAAEDEIaBzuE0b4OPHh2zgQATfaEwMNpxVnrIfaGpdQh2Tg_xyqZjPciEkA_8R_h4fHDlqkm-DPSIteMq-1wQVcinZQ_s", "XZaGwr2dyCZv", "BQAJAAEDEJbwpIepbc796kRNRvsfSeMgEt2HEO71RZl9eyMgvhYebbu8ujHAGcdWGg09w23z6LE.");
            netflixHandler.Users.RequestAvailableQueues();
            netflixHandler.Catalog.RequestCatalogTitles();
            
            /*
            var oauthHandler = new OAuthHandler();

            oauthHandler.Config.RequestTokenUri = new Uri(REQUEST_TOKEN_URL);
            oauthHandler.Config.AccessTokenUri = new Uri(ACCESS_TOKEN_URL);

            oauthHandler.Config.ApplicationName = "QuikPix for Netflix";

            oauthHandler.Config.OAuthConsumerKey = consumerKey;
            oauthHandler.Config.OAuthConsumerSecret = consumerSecret;
            var responseR = oauthHandler.GetRequestToken();
            string loginUrl = responseR["login_url"] + "&oauth_consumer_key=" + consumerKey;

            Process.Start(loginUrl);


            Console.WriteLine(responseR.ToString());
            Console.ReadLine();
            Console.WriteLine("--------------------");

            var responseA = oauthHandler.GetAccessToken();

            Console.WriteLine(responseA.ToString());
            Console.ReadLine();
            Console.WriteLine("--------------------");

            Console.WriteLine("Done");

            Console.ReadLine();
             */
        }

    }
}
