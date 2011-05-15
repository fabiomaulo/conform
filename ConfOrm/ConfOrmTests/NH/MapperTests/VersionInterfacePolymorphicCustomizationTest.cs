using System.Linq;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class VersionInterfacePolymorphicCustomizationTest
	{
		private class Entity
		{
			public int Id { get; set; }
		}

		private interface ITimeVersionedEntity
		{
			byte[] UpdatedAt { get; set; }
		}

		private interface IVersionedEntity
		{
			int Version { get; set; }
		}

		private class MyEntity : Entity, IVersionedEntity
		{
			public string Name { get; set; }

			public int Version { get; set; }
		}

		private class MyOtherEntity : Entity, ITimeVersionedEntity
		{
			public string Name { get; set; }

			public byte[] UpdatedAt { get; set; }
		}

		[Test]
		public void WhenCustomizedInterfaceThenWorkForConcreteImplementations()
		{
			var orm = new ObjectRelationalMapper();

			// Mark the version property using the interface
			orm.VersionProperty<IVersionedEntity>(x => x.Version);
			orm.VersionProperty<ITimeVersionedEntity>(x => x.UpdatedAt);

			var rootEntities = new[] { typeof(MyEntity), typeof(MyOtherEntity) };
			orm.TablePerClass(rootEntities);

			var mapper = new Mapper(orm);

			// customize base entities
			mapper.Class<IVersionedEntity>(cm => cm.Version(x => x.Version, vm => vm.Column("Revision")));

			mapper.Class<ITimeVersionedEntity>(cm => cm.Version(x => x.UpdatedAt, vm =>
			{
				vm.Column(column =>
				{
					column.Name("LastUpdate");
					column.NotNullable(true);
					column.SqlType("timestamp");
				});
				vm.Generated(VersionGeneration.Always);
			}));

			var mappings = mapper.CompileMappingFor(rootEntities);

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntity");
			hbmMyEntity.Version.Should().Not.Be.Null();
			hbmMyEntity.Version.name.Should().Be("Version");
			hbmMyEntity.Version.column1.Should().Be("Revision");

			HbmClass hbmMyOtherEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyOtherEntity");
			hbmMyOtherEntity.Version.Should().Not.Be.Null();
			hbmMyOtherEntity.Version.name.Should().Be("UpdatedAt");
			hbmMyOtherEntity.Version.Columns.Single().name.Should().Be("LastUpdate");
			hbmMyOtherEntity.Version.generated.Should().Be(HbmVersionGeneration.Always);
		}
	}
}