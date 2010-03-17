using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
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
			rc.Discriminator();
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
	}
}