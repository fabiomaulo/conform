using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class ManyToOnePatternsAddition
	{
		[Test]
		public void AddSimpleCustomDelegatedManyToOneApplierWithMember()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousManyToOneApplierCount = mapper.PatternsAppliers.ManyToOne.Count;

			mapper.AddManyToOnePattern(mi => true, cm => { });

			mapper.PatternsAppliers.ManyToOne.Count.Should().Be(previousManyToOneApplierCount + 1);
		}

		[Test]
		public void AddCustomDelegatedManyToOneApplierWithMember()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousManyToOneApplierCount = mapper.PatternsAppliers.ManyToOne.Count;

			mapper.AddManyToOnePattern(mi => true, (mi, cm) => { });

			mapper.PatternsAppliers.ManyToOne.Count.Should().Be(previousManyToOneApplierCount + 1);
		}
	}
}