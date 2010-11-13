using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class DiscriminatorIndexNameApplierTests
	{
		private class MyClass
		{
		}

		[Test]
		public void WhenEntityIsNotTablePerHierarchyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof (MyClass))).Returns(false);
			var applier = new DiscriminatorIndexNameApplier(orm.Object);
			applier.Match(typeof (MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenEntityIsTablePerHierarchyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof (MyClass))).Returns(true);
			var applier = new DiscriminatorIndexNameApplier(orm.Object);
			applier.Match(typeof (MyClass)).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyTheIndexNameWithSomeValue()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new DiscriminatorIndexNameApplier(orm.Object);

			var mapper = new Mock<IClassAttributesMapper>();
			var discriminatorMapper = new Mock<IDiscriminatorMapper>();
			var columnMapper = new Mock<IColumnMapper>();
			mapper.Setup(x => x.Discriminator(It.IsAny<Action<IDiscriminatorMapper>>())).Callback<Action<IDiscriminatorMapper>>(
				x => x.Invoke(discriminatorMapper.Object));
			discriminatorMapper.Setup(x => x.Column(It.IsAny<Action<IColumnMapper>>())).Callback<Action<IColumnMapper>>(
				x => x.Invoke(columnMapper.Object));

			applier.Apply(typeof (MyClass), mapper.Object);

			columnMapper.Verify(m => m.Index(It.Is<string>(c => c.Satisfy(value=> !string.IsNullOrEmpty(value)))));
		}
	}
}