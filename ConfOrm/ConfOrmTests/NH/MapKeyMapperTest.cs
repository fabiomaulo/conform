using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class MapKeyMapperTest
	{
		[Test]
		public void CatSetColumnByName()
		{
			var hbm = new HbmMapKey();
			var mapper = new MapKeyMapper(hbm);
			mapper.Column("pizza");
			hbm.column.Should().Be("pizza");
		}

		[Test]
		public void CatSetLength()
		{
			var hbm = new HbmMapKey();
			var mapper = new MapKeyMapper(hbm);
			mapper.Length(55);
			hbm.length.Should().Be("55");
		}
	}
}