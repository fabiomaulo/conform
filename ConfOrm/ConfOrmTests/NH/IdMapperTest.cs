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
		// The strategy Assigned is the default and does not need the "generator"
		//public void SetGeneratorAtCtor()
		//{
		//  var hbmId = new HbmId();
		//  new IdMapper(hbmId);
		//  hbmId.generator.Should().Not.Be.Null();
		//}

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

		[Test]
		public void CanSetGeneratorGuid()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Guid);
			hbmId.generator.@class.Should().Be.EqualTo("guid");
		}

		[Test]
		public void CanSetGeneratorGuidComb()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.GuidComb);
			hbmId.generator.@class.Should().Be.EqualTo("guid.comb");
		}

		[Test]
		public void CanSetGeneratorSequence()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Sequence);
			hbmId.generator.@class.Should().Be.EqualTo("sequence");
		}

		[Test]
		public void CanSetGeneratorIdentity()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Identity);
			hbmId.generator.@class.Should().Be.EqualTo("identity");
		}
	}
}