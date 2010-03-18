using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class UnionSubclassPatternsAddition
	{
		[Test]
		public void AddSimpleCustomDelegatedWithType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.UnionSubclass.Count;

			mapper.AddUnionSubclassPattern(mi => true, cm => { });

			mapper.PatternsAppliers.UnionSubclass.Count.Should().Be(previousApplierCount + 1);
		}

		[Test]
		public void AddCustomDelegatedWithoutType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.UnionSubclass.Count;

			mapper.AddUnionSubclassPattern(mi => true, (mi, cm) => { });

			mapper.PatternsAppliers.UnionSubclass.Count.Should().Be(previousApplierCount + 1);
		}
	}
}