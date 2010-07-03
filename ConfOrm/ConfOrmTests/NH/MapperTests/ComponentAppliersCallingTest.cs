using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class ComponentAppliersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyComponent Component { get; set; }
			public IEnumerable<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
			private MyClass _parent;
			public MyClass Parent
			{
				get { return _parent; }
			}
			public MyNestedComponent Component { get; set; }
		}

		private class MyNestedComponent
		{
			private MyComponent _parent;
			public MyComponent Parent
			{
				get { return _parent; }
			}
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(MyComponent) || t == typeof(MyNestedComponent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi.Name == "Components"))).Returns(true);
			return orm;
		}

		[Test]
		public void VerifyAppliersCalling()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			var applier = new Mock<IPatternApplier<Type, IComponentAttributesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<Type>())).Returns(true);

			mapper.PatternsAppliers.Component.Add(applier.Object);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<Type>(mi => mi == typeof(MyComponent))), Times.Exactly(2));
			applier.Verify(x => x.Apply(It.Is<Type>(mi => mi == typeof(MyComponent)), It.Is<IComponentAttributesMapper>(cm => cm != null)), Times.Exactly(2));

			applier.Verify(x => x.Match(It.Is<Type>(mi => mi == typeof(MyNestedComponent))), Times.Exactly(2));
			applier.Verify(x => x.Apply(It.Is<Type>(mi => mi == typeof(MyNestedComponent)), It.Is<IComponentAttributesMapper>(cm => cm != null)), Times.Exactly(2));
		}
	}
}