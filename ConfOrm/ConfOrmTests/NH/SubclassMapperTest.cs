using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class SubclassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class Inherited : EntitySimple
		{
		}

		[Test]
		public void WhenMapDocHasDefaultSubclassElementHasClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new SubclassMapper(subClass, mapdoc);
			mapdoc.SubClasses[0].Name.Should().Be.EqualTo(typeof (Inherited).Name);
		}

		[Test]
		public void WhenMapDocHasDefaultSubclassElementHasExtendsClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new SubclassMapper(subClass, mapdoc);
			mapdoc.SubClasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}
	}
}