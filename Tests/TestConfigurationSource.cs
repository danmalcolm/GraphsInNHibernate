using System;
using Network.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace Tests
{
    public class TestConfigurationSource
    {
        private static readonly Lazy<ISessionFactory> LazySessionFactory =
            new Lazy<ISessionFactory>(CreateSessionFactory);

        public static ISessionFactory SessionFactory
        {
            get { return LazySessionFactory.Value; }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var configuration = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.Dialect<MsSql2008Dialect>();
                    db.ConnectionStringName = "sqlexpress";
                    db.BatchSize = 100;
                    db.LogFormattedSql = true;
                    db.LogSqlInConsole = true;
                });
            new Mapping().ApplyTo(configuration);
            BuildSchema(configuration);
            return configuration.BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).Create(true, true);
        }
    }
}