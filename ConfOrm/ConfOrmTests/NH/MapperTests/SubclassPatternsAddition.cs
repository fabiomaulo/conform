using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class SubclassPatternsAddition
	{
		[Test]
		public void AddSimpleCustomDelegatedWithType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.Subclass.Count;

			mapper.AddSubclassPattern(mi => true, cm => { });

			mapper.PatternsAppliers.Subclass.Count.Should().Be(previousApplierCount + 1);
		}

		[Test]
		public void AddCustomDelegatedWithoutType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.Subclass.Count;

			mapper.AddSubclassPattern(mi => true, (mi, cm) => { });

			mapper.PatternsAppliers.Subclass.Count.Should().Be(previousApplierCount + 1);
		}
	}
}