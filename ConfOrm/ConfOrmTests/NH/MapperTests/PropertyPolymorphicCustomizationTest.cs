using System;
using System.Linq;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class PropertyPolymorphicCustomizationTest
	{
		private class Entity
		{
			public int Id { get; set; }
			public DateTime CreationDate { get; set; }
		}

		private class MyEntity : Entity
		{
			public string Name { get; set; }
		}

		[Test]
		public void WhenCustomizedBaseClassThenWorkInInherited()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			var mapper = new Mapper(orm);

			// customize base entities
			mapper.Customize<Entity>(cm => cm.Property(x => x.CreationDate, pm => pm.Update(false)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntity");
			var creationDateProp = hbmMyEntity.Properties.OfType<HbmProperty>().Single(p => p.Name == "CreationDate");
			creationDateProp.update.Should().Be.False();
			creationDateProp.updateSpecified.Should().Be.True();
		}
	}
}