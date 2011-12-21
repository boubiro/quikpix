using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate;

namespace QuikPix.Core
{
    public static class SessionFactory
    {
        public static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        .UsingFile("catalog.db")
                    )
                .Mappings(m =>
                    //m.UsePersistenceModel(
                    //    new PersistenceModel() { ValidationEnabled = false }
                    //)
                    m.FluentMappings.AddFromAssembly(typeof(SessionFactory).Assembly))
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration cfg)
        {
            if (!File.Exists("catalog.db"))
                new SchemaExport(cfg)
                    .Create(false, true);

            // People advice not to use NHibernate.Cache.HashtableCacheProvider for production
            cfg.SetProperty("cache.provider_class", "NHibernate.Cache.HashtableCacheProvider");
            cfg.SetProperty("cache.use_second_level_cache", "true");
            cfg.SetProperty("cache.use_query_cache", "true");
        }
    }
}
