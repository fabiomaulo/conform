using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class MapKeyManyToManyMapperTest
	{
		[Test]
		public void CatSetColumnByName()
		{
			var hbm = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(hbm);
			mapper.Column("pizza");
			hbm.column.Should().Be("pizza");
		}

		[Test]
		public void CatSetForeignKey()
		{
			var hbm = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(hbm);
			mapper.ForeignKey("pizza");
			hbm.foreignkey.Should().Be("pizza");
		}
	}
}