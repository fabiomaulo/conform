using System;
using ConfOrm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class DomainInspectorExtensionsTests
	{
		private class MainEntity
		{
			public int Id { get; set; }
		}
		private class InheritedEntity : MainEntity
		{
			public MyComponent Component { get; set; }
		}

		private class MyComponent
		{
			public int Something { get; set; }
		}

		[Test]
		public void WhenTypeIsNullThenReturnNull()
		{
			var orm = new Mock<IDomainInspector>();

			orm.Object.GetRootEntity(null).Should().Be.Null();
		}

		[Test]
		public void WhenInheritedEntityThenReturnRoot()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);

			orm.Object.GetRootEntity(typeof (InheritedEntity)).Should().Be(typeof (MainEntity));
		}

		[Test]
		public void WhenRootEntityThenReturnSameType()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);

			orm.Object.GetRootEntity(typeof(MainEntity)).Should().Be(typeof(MainEntity));
		}

		[Test]
		public void WhenNoEntityThenReturnNull()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(MyComponent))).Returns(false);

			orm.Object.GetRootEntity(typeof (MyComponent)).Should().Be.Null();
		}

		[Test]
		public void WhenNullDomainInspectorThenThrows()
		{
			Executing.This(() => typeof(InheritedEntity).GetRootEntity(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenInheritedEntityUsingTypeThenReturnRoot()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);

			typeof(InheritedEntity).GetRootEntity(orm.Object).Should().Be(typeof(MainEntity));
		}
	}
}