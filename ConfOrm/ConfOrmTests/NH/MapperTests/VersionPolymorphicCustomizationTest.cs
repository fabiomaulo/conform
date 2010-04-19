using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;
using ConfOrm.Mappers;

namespace ConfOrmTests.NH.MapperTests
{
	/// <summary>
	/// Show how work the polymorphic customization
	/// </summary>
	/// <remarks>
	/// Integration test
	/// </remarks>
	public class VersionPolymorphicCustomizationTest
	{
		private class Entity
		{
			public int Id { get; set; }
		}

		private class VersionedEntity : Entity
		{
			public int Version { get; set; }
		}

		private class MyEntity : VersionedEntity
		{
			public string Name { get; set; }
		}

		private class TimeVersionedEntity: Entity
		{
			public byte[] UpdatedAt { get; set; }
		}

		private class MyOtherEntity : TimeVersionedEntity
		{
			public string Name { get; set; }
		}

		[Test]
		public void WhenCustomizedBaseClassThenWorkInInherited()
		{
			var orm = new ObjectRelationalMapper();
			
			// Mark the version property using the base class
			orm.VersionProperty<VersionedEntity>(x => x.Version);
			orm.VersionProperty<TimeVersionedEntity>(x => x.UpdatedAt);

			var rootEntities = new[] { typeof(MyEntity),typeof(MyOtherEntity) };
			orm.TablePerClass(rootEntities);

			var mapper = new Mapper(orm);

			// customize base entities
			mapper.Class<VersionedEntity>(cm => cm.Version(x => x.Version, vm => vm.Column("Revision")));

			mapper.Class<TimeVersionedEntity>(cm => cm.Version(x => x.UpdatedAt, vm =>
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