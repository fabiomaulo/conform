using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ConfOrmExample.Domain;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ConfOrmExample
{
	public class NHIntegrationTest
	{
		private const string ConnectionString =
	@"Data Source=localhost\SQLEXPRESS;Initial Catalog=CONFORMTRY;Integrated Security=True;Pooling=False";

		public Configuration ConfigureNHibernate()
		{
			var configure = new Configuration();
			configure.SessionFactoryName("Demo");
			configure.Proxy(p =>
			{
				p.Validation = false;
				p.ProxyFactoryFactory<ProxyFactoryFactory>();
			});
			configure.DataBaseIntegration(db =>
			{
				db.Dialect<MsSql2008Dialect>();
				db.Driver<SqlClientDriver>();
				db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
				db.IsolationLevel = IsolationLevel.ReadCommitted;
				db.ConnectionString = ConnectionString;
				db.Timeout = 10;
				db.HqlToSqlSubstitutions = "true 1, false 0, yes 'Y', no 'N'";
			});
			return configure;
		}

		private HbmMapping GetMapping()
		{
			var domainMapper = new DefaultDomainMapper();
			// un-comment the follow line and comment the previous to run the demo with a different mapper
			//var domainMapper = new CoolDomainMapper();

			var entities = new List<Type>();

			entities.AddRange(ModuleMappingUtil.RunModuleMapping<NaturalnessModuleMapping>(domainMapper.DomainDefinition, domainMapper.Mapper));

			return domainMapper.Mapper.CompileMappingFor(entities);
		}

		[Test]
		public void SchemaExport()
		{
			Configuration conf = ConfigureNHibernate();
			conf.AddDeserializedMapping(GetMapping(), "Animals_Domain");
			SchemaMetadataUpdater.QuoteTableAndColumns(conf);
			Assert.DoesNotThrow(() => new SchemaExport(conf).Create(false, true));
			new SchemaExport(conf).Drop(false, true);
		}

		[Test]
		public void BuildSessionFactory()
		{
			Configuration conf = ConfigureNHibernate();
			conf.AddDeserializedMapping(GetMapping(), "Animals_Domain");
			SchemaMetadataUpdater.QuoteTableAndColumns(conf);
			new SchemaExport(conf).Create(false, true);
			Assert.DoesNotThrow(() => conf.BuildSessionFactory());
			new SchemaExport(conf).Drop(false, true);
		}

		[Test, Explicit]
		public void FullMappingProcessPerformance()
		{
			// The whole mapping process end when you have the session-factory ready to be used (create the first session)
			// NOTE: this method does not creates the Schema because it is a simulation of what happen in production

			var timer = Stopwatch.StartNew();

			Configuration conf = ConfigureNHibernate();
			conf.AddDeserializedMapping(GetMapping(), "Animals_Domain");
			var sessionFactoryReady = conf.BuildSessionFactory();

			timer.Stop();
			Console.WriteLine("The whole mapping-process took {0}.{1} seconds.", timer.Elapsed.Seconds,timer.Elapsed.Milliseconds);
		}

		[Test]
		public void JustForFun()
		{
			Configuration conf = ConfigureNHibernate();
			conf.AddDeserializedMapping(GetMapping(), "Animals_Domain");
			SchemaMetadataUpdater.QuoteTableAndColumns(conf);
			new SchemaExport(conf).Create(false, true);
			ISessionFactory factory = conf.BuildSessionFactory();

			using (ISession s = factory.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var polliwog = new Animal { BodyWeight = 12, Description = "Polliwog" };

					var catepillar = new Animal { BodyWeight = 10, Description = "Catepillar" };

					var frog = new Animal { BodyWeight = 34, Description = "Frog" };

					polliwog.Father = frog;
					frog.AddOffspring(polliwog);

					var butterfly = new Animal { BodyWeight = 9, Description = "Butterfly" };

					catepillar.Mother = butterfly;
					butterfly.AddOffspring(catepillar);

					s.Save(frog);
					s.Save(polliwog);
					s.Save(butterfly);
					s.Save(catepillar);

					var dog = new Dog { BodyWeight = 200, Description = "dog" };
					s.Save(dog);

					var cat = new Cat { BodyWeight = 100, Description = "cat" };
					s.Save(cat);

					var zoo = new Zoo { Name = "Zoo" };
					var add = new Address { City = "MEL", Country = "AU", Street = "Main st", PostalCode = "3000" };
					zoo.Address = add;

					Zoo pettingZoo = new PettingZoo { Name = "Petting Zoo" };
					var addr = new Address { City = "Sydney", Country = "AU", Street = "High st", PostalCode = "2000" };
					pettingZoo.Address = addr;

					s.Save(zoo);
					s.Save(pettingZoo);
					tx.Commit();
				}
			}

			using (ISession s = factory.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.CreateQuery("delete from Animal where mother is not null or father is not null").ExecuteUpdate();
					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.CreateQuery("delete from Zoo").ExecuteUpdate();
					tx.Commit();
				}
			}

			new SchemaExport(conf).Drop(false, true);
		}

	}
}