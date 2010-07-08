using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class DiscriminatorColumnNameApplierTests
	{
		private class MyClass
		{
		}

		[Test]
		public void WhenEntityIsNotTablePerHierarchyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof (MyClass))).Returns(false);
			var applier = new DiscriminatorColumnNameApplier(orm.Object);
			applier.Match(typeof (MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenEntityIsTablePerHierarchyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof (MyClass))).Returns(true);
			var applier = new DiscriminatorColumnNameApplier(orm.Object);
			applier.Match(typeof (MyClass)).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyEntityNameAsColumn()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new DiscriminatorColumnNameApplier(orm.Object);

			var mapper = new Mock<IClassAttributesMapper>();
			var discriminatorMapper = new Mock<IDiscriminatorMapper>();
			mapper.Setup(x => x.Discriminator(It.IsAny<Action<IDiscriminatorMapper>>())).Callback<Action<IDiscriminatorMapper>>(
				x => x.Invoke(discriminatorMapper.Object));

			applier.Apply(typeof (MyClass), mapper.Object);

			discriminatorMapper.Verify(m => m.Column(It.Is<string>(c => "EntityType".Equals(c))));
		}
	}
}