using System;
using QuikPix.Core;

namespace QuixPix.CatalogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessionFactory = SessionFactory.CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                var catalogParser = new QuikPix.Core.Catalog.Parser.CatalogParser(session);

                using (var transaction = session.BeginTransaction())
                {
                    var catalog = catalogParser.ParseCatalog("full_catalog.xml");

                    //var genre1 = new Genre("1", "Genre 1");
                    //var genre2 = new Genre("2", "Genre 2");
                    //var genre3 = new Genre("3", "Genre 3");
                    //var genre4 = new Genre("4", "Genre 4");

                    //genre1.ChildGenres.Add(genre3);
                    //genre1.ChildGenres.Add(genre4);

                    //genre2.ChildGenres.Add(genre3);

                    //genre3.ParentGenres.Add(genre1);
                    //genre3.ParentGenres.Add(genre2);

                    //genre4.ParentGenres.Add(genre1);

                    //session.SaveOrUpdate(genre1);
                    //session.SaveOrUpdate(genre2);
                    //session.SaveOrUpdate(genre3);
                    //session.SaveOrUpdate(genre4);

                    //foreach (var title in catalog)
                    //{
                    //    Console.WriteLine(title.RegularTitle + ": " + string.Join(",", title.Genres.Select(x => x.Label).ToArray()));
                    //    session.SaveOrUpdate(title);
                    //}
                    //session.SaveOrUpdate(catalog.First());
                    try
                    {
                        transaction.Commit();
                    } catch (Exception ex)  {
                        throw ex;
                    }
                }

                //var g1 = session.Get<Genre>("1");
                //var g2 = session.Get<Genre>("3");
            }
            //Console.ReadKey();
        }
    }
}
