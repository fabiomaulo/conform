using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class JoinedSubclassPatternsAddition
	{
		[Test]
		public void AddSimpleCustomDelegatedWithType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.JoinedSubclass.Count;

			mapper.AddJoinedSubclassPattern(mi => true, cm => { });

			mapper.PatternsAppliers.JoinedSubclass.Count.Should().Be(previousApplierCount + 1);
		}

		[Test]
		public void AddCustomDelegatedWithoutType()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousApplierCount = mapper.PatternsAppliers.JoinedSubclass.Count;

			mapper.AddJoinedSubclassPattern(mi => true, (mi, cm) => { });

			mapper.PatternsAppliers.JoinedSubclass.Count.Should().Be(previousApplierCount + 1);
		}
	}
}