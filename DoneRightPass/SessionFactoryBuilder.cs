using DoneRightPass.Model;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace DoneRightPass
{
	public class SessionFactoryBuilder
	{
		public static ISessionFactory BuildSessionFactory()
		{
			return Fluently.Configure()
				.Database(SQLiteConfiguration
							.Standard
							.UsingFile("Vault.db")
							.ShowSql
				)
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<Form1>())
				.ExposeConfiguration(c => new SchemaExport(c)
						 .Create(false, true))
				.BuildSessionFactory();

		}
	}
}