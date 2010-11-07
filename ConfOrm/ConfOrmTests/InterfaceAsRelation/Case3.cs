using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case3
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IHasMessage TheComponent { get; set; }
		}

		private interface IHasMessage
		{
			string Message { get; set; }
		}

		private class MyComponent : IHasMessage
		{
			public string Message { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeManyToOneWithTheCorrectClass()
		{
			// Note: the MyComponent class must be explicitly declared in ORM or mapper.CompileMappingFor

			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			orm.AddToDomain(typeof(MyComponent));

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			rc.Properties.Where(p => p.Name == "TheComponent").Single().Should().Be.OfType<HbmComponent>()
				.And.ValueOf.Class.Should().Contain("MyComponent");
		}
	}
}