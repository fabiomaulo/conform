using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class IdMapperTest
	{
		[Test]
		public void SetGeneratorAtCtor()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId);
			hbmId.generator.Should().Not.Be.Null();
		}

		[Test]
		public void CanSetGenerator()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId) { Generator = Generators.HighLow };
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
		}
	}
}