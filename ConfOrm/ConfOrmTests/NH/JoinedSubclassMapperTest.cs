using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class JoinedSubclassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class Inherited : EntitySimple
		{
		}

		[Test]
		public void WhenMapDocHasDefaultJoinedSubclassElementHasClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new JoinedSubclassMapper(subClass, mapdoc);
			mapdoc.JoinedSubclasses[0].Name.Should().Be.EqualTo(typeof(Inherited).Name);
		}

		[Test]
		public void WhenMapDocHasDefaultJoinedSubclassElementHasExtendsClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new JoinedSubclassMapper(subClass, mapdoc);
			mapdoc.JoinedSubclasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}
	}
}