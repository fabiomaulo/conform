using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyInverseApplierTests
	{
		private class Parent
		{
			public int Id { get; set; }
			public ICollection<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public Parent Owner { get; set; }
		}

		[Test]
		public void WhenChildIsEntityThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent) || t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Parent) || t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);

			var applier = new BidirectionalOneToManyInverseApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.True();
		}

		[Test]
		public void WhenChildIsNotEntityThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);

			var applier = new BidirectionalOneToManyInverseApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.False();
		}
	}
}