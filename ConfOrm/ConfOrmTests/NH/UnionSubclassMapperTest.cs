using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class UnionSubclassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class Inherited : EntitySimple
		{
		}

		[Test]
		public void WhenMapDocHasDefaultUnionSubclassElementHasClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new UnionSubclassMapper(subClass, mapdoc);
			mapdoc.UnionSubclasses[0].Name.Should().Be.EqualTo(typeof(Inherited).Name);
		}

		[Test]
		public void WhenMapDocHasDefaultUnionSubclassElementHasExtendsClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new UnionSubclassMapper(subClass, mapdoc);
			mapdoc.UnionSubclasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}
	}
}