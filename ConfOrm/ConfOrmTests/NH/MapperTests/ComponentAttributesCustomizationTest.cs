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
	public class ComponentAttributesCustomizationTest
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
		}
		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t != typeof(MyComponent)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(MyComponent)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(MyComponent)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi.Name == "Components"))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenCustomizeComponentThenApplyCustomizationToMapping()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Component<MyComponent>(ca => ca.Parent(mycomponent => mycomponent.Parent));
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var hbmComponent = (HbmComponent)rc.Properties.First(p => p.Name == "Component");
			hbmComponent.Parent.name.Should().Be("Parent");
		}

		[Test]
		public void WhenCustomizeComponentParentAccessThenApplyCustomizationToMapping()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Component<MyComponent>(ca => ca.Parent(mycomponent => mycomponent.Parent, pm=> pm.Access(Accessor.Field)));
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var hbmComponent = (HbmComponent)rc.Properties.First(p => p.Name == "Component");
			hbmComponent.Parent.name.Should().Be("Parent");
			hbmComponent.Parent.access.Should().Be("field.camelcase-underscore");
		}

		[Test]
		public void WhenCustomizeComponentParentAccessThenApplyCustomizationToMappingForCompositeElement()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Component<MyComponent>(ca => ca.Parent(mycomponent => mycomponent.Parent, pm => pm.Access(Accessor.Field)));
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var hbmBag = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var hbmCompositeElement = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmCompositeElement.Parent.name.Should().Be("Parent");
			hbmCompositeElement.Parent.access.Should().Be("field.camelcase-underscore");
		}

		[Test]
		public void WhenCustomizeComponentParentAccessThroughCollectionThenApplyCustomizationToMappingForCompositeElement()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Class<MyClass>(
				ca =>
				ca.Bag(mycomponent => mycomponent.Components, x => { },
				       cer => cer.Component(c => c.Parent(mycomponent => mycomponent.Parent, pa => pa.Access(Accessor.Field)))));

			HbmMapping mapping = mapper.CompileMappingFor(new[] {typeof (MyClass)});

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var hbmBag = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var hbmCompositeElement = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmCompositeElement.Parent.name.Should().Be("Parent");
			hbmCompositeElement.Parent.access.Should().Be("field.camelcase-underscore");
		}
	}
}