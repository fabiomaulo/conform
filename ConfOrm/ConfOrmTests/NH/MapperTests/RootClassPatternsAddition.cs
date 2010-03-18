using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class RootClassPatternsAddition
	{
		[Test]
		public void AddSimpleCustomDelegatedWithType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.RootClass.Count;

			mapper.AddRootClassPattern(mi => true, cm => { });

			mapper.PatternsAppliers.RootClass.Count.Should().Be(previousApplierCount + 1);
		}

		[Test]
		public void AddCustomDelegatedWithoutType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.RootClass.Count;

			mapper.AddRootClassPattern(mi => true, (mi, cm) => { });

			mapper.PatternsAppliers.RootClass.Count.Should().Be(previousApplierCount + 1);
		}
	}
}