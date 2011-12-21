using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;
using QuikPix.Core.Catalog;
using NHibernate.Criterion;
using NHibernate.Linq;
using QuikPix.Core.TransferObjects;
using NHibernate.Transform;
using NHibernate.SqlCommand;
using System.Xml.Linq;
using System.Net;
using System.Diagnostics;

namespace QuikPix.Core
{
    public class QuikPixCore
    {
        private const string ConsumerKey = "nvcgk2wk6zsukejaeknbs7ay";
        
        #region Singleton

        private static QuikPixCore instance = null;
        private static Object instanceLock = new Object();


        public static QuikPixCore Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new QuikPixCore();
                }
                return instance;
            }
        }

        #endregion

        private readonly ISessionFactory sessionFactory = null;
        private readonly IList<Genre> genres;
        private readonly IList<Title> titles;


        private QuikPixCore()
        {
            sessionFactory = SessionFactory.CreateSessionFactory();

            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    //genres = session.CreateCriteria<Genre>()
                    //    .AddOrder(Order.Desc("RootGenre"))
                    //    .AddOrder(Order.Asc("Label"))
                    //    .List<Genre>();

                    //titles = session.CreateCriteria<Title>()
                    //    .AddOrder(Order.Desc("AverageRating"))
                    //    .List<Title>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class TestGenre
        {
            public virtual string GenreId { get; set; }
            public virtual string Label { get; set; }
            public virtual decimal AverageRating { get; set; }
            public virtual string TitleId { get; set; }
            public virtual string Title { get; set; }
            public virtual string BoxArt { get; set; }
        }

        private IList<GenreDisplayItem> _rootGenres;
        public IEnumerable<GenreDisplayItem> GetRootGenres()
        {
            try
            {
                if (_rootGenres == null || _rootGenres.Count == 0)
                {
                    using (var session = sessionFactory.OpenSession())
                    {
                        _rootGenres = session.CreateCriteria<Genre>()
                            .Add(Expression.Eq("RootGenre", true))
                            .AddOrder(Order.Asc("Label"))
                            .List<Genre>()
                            .Select(g => new GenreDisplayItem(g)).ToList();
                    }
                }
                return _rootGenres;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<GenreDisplayItem> GetSubGenres(string parentGenreId)
        {
            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    return session.CreateCriteria<Genre>()
                        .CreateAlias("ParentGenres", "pg")
                        .Add(Expression.Eq("RootGenre", false))
                        .Add(Expression.Eq("pg.GenreId", parentGenreId))
                        .AddOrder(Order.Asc("Label"))
                        .List<Genre>()
                        .Select(g => new GenreDisplayItem(g)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<TitleDisplayItem> GetTitles(string parentGenreId)
        {
            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    return session.CreateCriteria<Title>()
                        .CreateAlias("Genres", "g")
                        .Add(Expression.Eq("g.GenreId", parentGenreId))
                        .AddOrder(Order.Desc("AverageRating"))
                        .SetMaxResults(250)
                        .List<Title>()
                        .Select(t => new TitleDisplayItem(t)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Title GetTitle(string titleId)
        {
            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    return session.Get<Title>(titleId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Autocomplete(string term)
        {
            try {
            var autocompleteUri = string.Format("http://api.netflix.com/catalog/titles/autocomplete?oauth_consumer_key={0}&term={1}", ConsumerKey, term);

            HttpWebRequest request = HttpWebRequest.Create(autocompleteUri) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            var xdoc = XDocument.Load(response.GetResponseStream());

            var xbase = xdoc.Element("autocomplete");
            if (xbase != null && xbase.HasElements)
            {
                return xbase.Elements("autocomplete_item")
                    .Select(x => x.Element("title").Attribute("short").Value).ToArray();
            }
            } catch (Exception ex) {
                Debug.WriteLine(ex, "Autocomplete Error");
            }
            return new string[] { };
        }
    }
}
