using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class AnyIntegrationTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public object MyReferenceClass { get; set; }
		}

		[Test]
		public void ContainHbmAnyElement()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.HeterogeneousAssociation<MyClass>(mc => mc.MyReferenceClass);
			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] {typeof (MyClass)});
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.Properties.Single(p => p.Name == "MyReferenceClass").Should().Be.OfType<HbmAny>();
		}
	}
}