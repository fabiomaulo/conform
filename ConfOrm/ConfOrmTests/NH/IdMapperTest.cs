using System.Linq;
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
			new IdMapper(hbmId).Generator(Generators.HighLow);
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
		}

		[Test]
		public void CanSetGeneratorWithParameters()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.HighLow, p => p.Params(new {max_low = 99, where = "TableName"}));
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
			hbmId.generator.param.Should().Have.Count.EqualTo(2);
			hbmId.generator.param.Select(p => p.name).Should().Have.SameValuesAs("max_low", "where");
			hbmId.generator.param.Select(p => p.GetText()).Should().Have.SameValuesAs("99", "TableName");
		}
	}
}