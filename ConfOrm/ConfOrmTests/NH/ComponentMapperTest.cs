using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ComponentMapperTest
	{
		private class Person
		{
			public int Id { get; set; }
			public Name Name { get; set; }
		}

		private class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
		}

		[Test]
		public void WhenMapDocHasDefaultHasClassName()
		{
			var entityType = typeof (Person);
			var mapdoc = new HbmMapping { assembly = entityType.Assembly.FullName, @namespace = entityType.Namespace };
			var component = new HbmComponent();
			new ComponentMapper(component, typeof(Name), mapdoc);
			component.Class.Should().Be.EqualTo(typeof(Name).Name);
		}
	}
}