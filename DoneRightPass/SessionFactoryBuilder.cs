using System;
using DoneRightPass.Model;


namespace DoneRightPass
{
	/*public class ModelConfig : DefaultAutomappingConfiguration
	{
		public override bool ShouldMap(Type type)
		{
			return type.Namespace == "DoneRightPass.Model";
		}

		public override bool IsId(Member member)
		{
			return member.Name == "Id";
		}
	}

	public class SessionFactoryBuilder
	{
		public static ISessionFactory BuildSessionFactory()
		{
			var cfg = new ModelConfig();
			return Fluently.Configure()
				.Database(SQLiteConfiguration
					.Standard
					.UsingFile("Vault.db")
					.ShowSql
				)
				.Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<RecordType>(cfg)))
				.ExposeConfiguration(c => new SchemaExport(c)
					.Create(false, true))
				.BuildSessionFactory();
		}
	}*/
}