using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelation Relation { get; set; }
			public Relation1 Relation1 { get; set; }
		}

		private interface IRelation
		{
		}

		private class MyRelation : IRelation
		{
			public int Id { get; set; }
		}

		private class Relation1
		{
		}

		private class MyRelation1 : Relation1
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeManyToOneWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyRelation1>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			rc.Properties.Where(p => p.Name == "Relation").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Contain("MyRelation");
			rc.Properties.Where(p => p.Name == "Relation1").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Contain("MyRelation1");
		}
	}
}