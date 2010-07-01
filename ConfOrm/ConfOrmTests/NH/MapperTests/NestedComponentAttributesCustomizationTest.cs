using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class NestedComponentAttributesCustomizationTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public IEnumerable<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
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
		public void WhenCustomizeNestedComponentParentAccessThroughCollectionThenApplyCustomizationToMappingForCompositeElement()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Class<MyClass>(
				ca =>
				ca.Bag(mycomponent => mycomponent.Components, x => { },
				       cer =>
				       cer.Component(
				       	c =>
				       	c.Component(mycomponent => mycomponent.Component,
				       	            ce => ce.Parent(mync => mync.Parent, pa => pa.Access(Accessor.Field))))));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var hbmBag = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var hbmCompositeElement = (HbmCompositeElement)hbmBag.ElementRelationship;
			var hbmNestedCompositeElement = (HbmNestedCompositeElement)hbmCompositeElement.Properties.First(p => p.Name == "Component");
			hbmNestedCompositeElement.Parent.name.Should().Be("Parent");
			hbmNestedCompositeElement.Parent.access.Should().Be("field.camelcase-underscore");
		}

	}
}