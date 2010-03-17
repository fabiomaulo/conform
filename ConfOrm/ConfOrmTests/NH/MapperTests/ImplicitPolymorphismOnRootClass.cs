using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class ImplicitPolymorphismOnRootClass
	{
		private class EntityBase
		{
			public int Id { get; set; }
		}

		private class MyBaseClass: EntityBase
		{
			public string BaseProp { get; set; }
		}

		private class MyClass : MyBaseClass
		{
			public string MyProperty { get; set; }
		}

		[Test]
		public void WhenRegisteredAsRootThenTakeFlatHierarchyProperties()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmMyClass = mappings.RootClasses.Single();
			hbmMyClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("BaseProp", "MyProperty");
		}

		[Test]
		public void WhenNotRegisteredAsRootThenTakePropertiesOfTheClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyBaseClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(MyBaseClass), typeof(MyClass) });

			var hbmMyBaseClass = mappings.RootClasses.Single();
			hbmMyBaseClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("BaseProp");
			var hbmMyClass = mappings.JoinedSubclasses.Single();
			hbmMyClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("MyProperty");
		}
	}
}