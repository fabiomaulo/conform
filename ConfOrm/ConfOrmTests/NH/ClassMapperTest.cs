using System.Linq;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ClassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class EntitySimpleWithVersion
		{
			public int Id { get; set; }
			public int EntityVersion { get; set; }
		}

		private class EntitySimpleWithNaturalId
		{
			public int Id { get; set; }
			public int Code { get; set; }
			public string Name { get; set; }
		}

		private interface IEntityProxable
		{
			int Id { get; set; }
			int Code { get; set; }
		}

		private interface IAnotherInterface
		{
			int Code { get; set; }
		}

		private class EntityProxable : IEntityProxable
		{
			public int Id { get; set; }
			public int Code { get; set; }
		}

		[Test]
		public void AddClassElementToMappingDocument()
		{
			var mapdoc = new HbmMapping();
			new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			mapdoc.RootClasses.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void ClassElementHasName()
		{
			var mapdoc = new HbmMapping();
			new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			mapdoc.RootClasses[0].Name.Should().Not.Be.Null();
		}

		[Test]
		public void ClassElementHasIdElement()
		{
			var mapdoc = new HbmMapping();
			new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			var hbmId = mapdoc.RootClasses[0].Id;
			hbmId.Should().Not.Be.Null();
			hbmId.name.Should().Be.EqualTo("Id");
		}

		[Test]
		public void ClassElementHasIdTypeElement()
		{
			var mapdoc = new HbmMapping();
			new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			var hbmId = mapdoc.RootClasses[0].Id;
			var hbmType = hbmId.Type;
			hbmType.Should().Not.Be.Null();
			hbmType.name.Should().Contain("Int32");
		}

		[Test]
		public void CanSetDistriminator()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Discriminator(x => { });
			mapdoc.RootClasses[0].discriminator.Should().Not.Be.Null();
		}

		[Test]
		public void WhenMapDocHasDefaultClassElementHasClassName()
		{
			var rootClass = typeof (EntitySimple);
			var mapdoc = new HbmMapping {assembly = rootClass.Assembly.FullName, @namespace = rootClass.Namespace};
			new ClassMapper(rootClass, mapdoc, rootClass.GetProperty("Id"));
			mapdoc.RootClasses[0].Name.Should().Be.EqualTo("EntitySimple");
		}

		[Test]
		public void WhenSetDistriminatorValueOnlySetValueAndType()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.DiscriminatorValue(123);
			mapdoc.RootClasses[0].discriminatorvalue.Should().Be("123");
			mapdoc.RootClasses[0].discriminator.Should().Not.Be.Null();
			mapdoc.RootClasses[0].discriminator.type.Should().Contain("Int32");
		}

		[Test]
		public void CanSetTable()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Table("pizza");
			mapdoc.RootClasses[0].table.Should().Be("pizza");
		}

		[Test]
		public void CanSetCatalog()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Catalog("pizza");
			mapdoc.RootClasses[0].catalog.Should().Be("pizza");
		}

		[Test]
		public void CanSetSchema()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Schema("pizza");
			mapdoc.RootClasses[0].schema.Should().Be("pizza");
		}

		[Test]
		public void CanSetMutable()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Mutable(false);
			mapdoc.RootClasses[0].mutable.Should().Be.False();			
		}

		[Test]
		public void CanSetVersion()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimpleWithVersion), mapdoc, typeof(EntitySimpleWithVersion).GetProperty("Id"));
			rc.Version(typeof(EntitySimpleWithVersion).GetProperty("EntityVersion"), vm => vm.Generated(VersionGeneration.Always));
			var hbmVersion = mapdoc.RootClasses[0].Version;
			hbmVersion.Should().Not.Be.Null();
			hbmVersion.generated.Should().Be(HbmVersionGeneration.Always);
		}

		[Test]
		public void WhenSetTwoVersionPropertiesInTwoActionThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimpleWithVersion), mapdoc, typeof(EntitySimpleWithVersion).GetProperty("Id"));
			rc.Version(typeof(EntitySimpleWithVersion).GetProperty("EntityVersion"), vm => vm.Generated(VersionGeneration.Always));
			rc.Version(typeof(EntitySimpleWithVersion).GetProperty("EntityVersion"), vm => vm.Column("pizza"));
			var hbmVersion = mapdoc.RootClasses[0].Version;
			hbmVersion.generated.Should().Be(HbmVersionGeneration.Always);
			hbmVersion.column1.Should().Be("pizza");
		}

		[Test]
		public void CanSetNaturalId()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimpleWithNaturalId), mapdoc, typeof(EntitySimpleWithNaturalId).GetProperty("Id"));
			rc.NaturalId(nidm => nidm.Property(typeof(EntitySimpleWithNaturalId).GetProperty("Code"), pm => { }));
			
			mapdoc.RootClasses[0].Properties.Should("The property should be only inside natural-id").Have.Count.EqualTo(0);
			
			var hbmNaturalId = mapdoc.RootClasses[0].naturalid;
			hbmNaturalId.Should().Not.Be.Null();
			hbmNaturalId.Properties.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetTwoNaturalIdPropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimpleWithNaturalId), mapdoc, typeof(EntitySimpleWithNaturalId).GetProperty("Id"));
			rc.NaturalId(nidm => nidm.Property(typeof(EntitySimpleWithNaturalId).GetProperty("Code"), pm => { }));
			rc.NaturalId(nidm => nidm.Property(typeof(EntitySimpleWithNaturalId).GetProperty("Name"), pm => { }));
			rc.NaturalId(nidm => nidm.Mutable(true));

			mapdoc.RootClasses[0].Properties.Should("The property should be only inside natural-id").Have.Count.EqualTo(0);

			var hbmNaturalId = mapdoc.RootClasses[0].naturalid;
			hbmNaturalId.Should().Not.Be.Null();
			hbmNaturalId.mutable.Should().Be.True();
			hbmNaturalId.Properties.Should().Have.Count.EqualTo(2);
		}

		[Test]
		public void CallSetCache()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Cache(ch=> ch.Region("pizza"));
			mapdoc.RootClasses[0].cache.Should().Not.Be.Null();
		}

		[Test]
		public void CallSetDiscriminator()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.DiscriminatorValue("X");
			mapdoc.RootClasses[0].discriminator.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimpleWithNaturalId), mapdoc, typeof(EntitySimpleWithNaturalId).GetProperty("Id"));
			rc.Cache(ch => ch.Region("pizza"));
			rc.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = mapdoc.RootClasses[0].cache;
			hbmCache.Should().Not.Be.Null();
			hbmCache.region.Should().Be("pizza");
			hbmCache.usage.Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}

		[Test]
		public void CanSetAFilterThroughAction()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Filter("filter1", f => f.Condition("condition1"));
			mapdoc.RootClasses[0].filter.Length.Should().Be(1);
			mapdoc.RootClasses[0].filter[0].Satisfy(f => f.name == "filter1" && f.condition == "condition1");
		}

		[Test]
		public void CanSetMoreFiltersThroughAction()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Filter("filter1", f => f.Condition("condition1"));
			rc.Filter("filter2", f => f.Condition("condition2"));
			mapdoc.RootClasses[0].filter.Length.Should().Be(2);
			mapdoc.RootClasses[0].filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "condition1"));
			mapdoc.RootClasses[0].filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenSameNameThenOverrideCondition()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Filter("filter1", f => f.Condition("condition1"));
			rc.Filter("filter2", f => f.Condition("condition2"));
			rc.Filter("filter1", f => f.Condition("anothercondition1"));
			mapdoc.RootClasses[0].filter.Length.Should().Be(2);
			mapdoc.RootClasses[0].filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "anothercondition1"));
			mapdoc.RootClasses[0].filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenActionIsNullThenAddFilterName()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Filter("filter1", null);
			mapdoc.RootClasses[0].filter.Length.Should().Be(1);
			mapdoc.RootClasses[0].filter[0].Satisfy(f => f.name == "filter1" && f.condition == null);
		}

		[Test]
		public void CanSetWhereClause()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.Where("Id > 0");
			mapdoc.RootClasses[0].where.Should().Be("Id > 0");
		}

		[Test]
		public void CanSetSchemaAction()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, typeof(EntitySimple).GetProperty("Id"));
			rc.SchemaAction(SchemaAction.None);
			mapdoc.RootClasses[0].schemaaction.Should().Be("none");
		}

		[Test]
		public void CanSetProxy()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntityProxable), mapdoc, ConfOrm.ForClass<EntityProxable>.Property(x => x.Id));
			rc.Proxy(typeof(IEntityProxable));

			var hbmEntity = mapdoc.RootClasses[0];
			hbmEntity.Proxy.Should().Contain("IEntityProxable");
		}

		[Test]
		public void WhenSetWrongProxyThenThrow()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntityProxable), mapdoc, ConfOrm.ForClass<EntityProxable>.Property(x => x.Id));
			rc.Executing(m => m.Proxy(typeof(IAnotherInterface))).Throws<MappingException>();
		}

		[Test]
		public void SetSqlSubselect()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(EntityProxable), mapdoc, ConfOrm.ForClass<EntityProxable>.Property(x => x.Id));
			mapper.Subselect("blah");

			var hbmEntity = mapdoc.RootClasses[0];

			hbmEntity.Subselect.Should().Not.Be.Null();
			hbmEntity.subselect.Text[0].Should().Be("blah");
		}
	}
}