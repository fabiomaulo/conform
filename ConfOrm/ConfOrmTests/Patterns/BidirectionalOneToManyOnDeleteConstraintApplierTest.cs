using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyOnDeleteConstraintApplierTest
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

			var applier = new BidirectionalOneToManyOnDeleteConstraintApplier(orm.Object);
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

			var applier = new BidirectionalOneToManyOnDeleteConstraintApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.False();
		}

		[Test]
		public void WhenChildIsTablePerClassNoRootEntityThenNoMatch()
		{
			// Suppose this relation mapped as TablePerClass: 
			// Person, Contact : Person, Vendor : Contact
			// then we have the class Company with a bidirectional-one-to-many with Vendor and we want a cascade (deleting Company it should delete Vendors).
			// The trigger will delete the record in the Vendor table leaving records in Person and in Contact.
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);

			var applier = new BidirectionalOneToManyOnDeleteConstraintApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.False();
		}

		[Test]
		public void WhenChildIsTablePerClassRootEntityThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);

			var applier = new BidirectionalOneToManyOnDeleteConstraintApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.True();
		}

		[Test]
		public void WhenChildIsNotTablePerClassNoRootEntityThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsTablePerClassHierarchy(It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);

			var applier = new BidirectionalOneToManyOnDeleteConstraintApplier(orm.Object);
			applier.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.True();
		}
	}
}