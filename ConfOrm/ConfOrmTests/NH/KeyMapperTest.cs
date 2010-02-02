using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class KeyMapperTest
	{
		private class Animal
		{
			
		}

		[Test]
		public void AutoAssignColumnNameByDefault()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof (Animal), hbm);
			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.First().name.Should().Not.Be.Empty().And.Not.Be.Null();
		}

		[Test]
		public void WhenExplicitAssignementThenOverrideDefaultColumnName()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.Column("blha");
			hbm.Columns.First().name.Should().Be.EqualTo("blha");
		}
	}
}