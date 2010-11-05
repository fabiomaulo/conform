using System;
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
		[Test, Ignore("Not 'auto' supported.")]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeManyToOneWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			rc.Properties.Where(p => p.Name == "Relation").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Contain("MyRelation");
		}
	}
}