using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests.NestedComponetInCollectionCustomization
{
	public class CustomizationThroughCollection
	{
		private class MyClass
		{
			public int Id { get; set; }
			public IEnumerable<ComponentLevel0> Components { get; set; }
		}

		private class ComponentLevel0
		{
			private ComponentLevel1 componentLevel1;
			public string Something0 { get; set; }

			public ComponentLevel1 ComponentLevel1
			{
				get { return componentLevel1; }
			}
		}

		private class ComponentLevel1
		{
			private ComponentLevel2 componentLevel2;
			public string Something1 { get; set; }

			public ComponentLevel2 ComponentLevel2
			{
				get { return componentLevel2; }
			}
		}

		private class ComponentLevel2
		{
			public string Something2 { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(mc => mc.Components)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel0)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel1)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel2)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenCustomizeNestedCompositeElementPropertiesThenExecuteTheCustomizationOnTheSpecificMemberPath()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Class<MyClass>(map=> map.Bag(myClass=> myClass.Components,x=> { },
				rel=> rel.Component(cm => cm.Component(myclass => myclass.ComponentLevel1, compo => compo.Property(cl1 => cl1.Something1, m => m.Column("MyColName"))))));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var collection = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var relation = (HbmCompositeElement)collection.ElementRelationship;
			relation.Should().Be.OfType<HbmCompositeElement>();
			var component = (HbmNestedCompositeElement)relation.Properties.First(p => p.Name == "ComponentLevel1");
			var property = (HbmProperty)component.Properties.First(p => p.Name == "Something1");
			property.column.Should().Be("MyColName");
		}

		[Test]
		public void WhenCustomizeNestedCompositeElementPropertiesAt3thLevelThenExecuteTheCustomizationOnTheSpecificMemberPath()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Class<MyClass>(map=> map.Bag(myClass=> myClass.Components,x=> { },
				rel=> rel.Component(cm =>
				cm.Component(myclass => myclass.ComponentLevel1, 
					c1m=> c1m.Component(c1 => c1.ComponentLevel2, compo => compo.Property(cl1 => cl1.Something2, m => m.Column("MyColName"))))
				)));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var collection = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var relation = (HbmCompositeElement)collection.ElementRelationship;
			relation.Should().Be.OfType<HbmCompositeElement>();
			var component1 = (HbmNestedCompositeElement)relation.Properties.First(p => p.Name == "ComponentLevel1");
			var component2 = (HbmNestedCompositeElement)component1.Properties.First(p => p.Name == "ComponentLevel2");
			var property = (HbmProperty)component2.Properties.First(p => p.Name == "Something2");
			property.column.Should().Be("MyColName");
		}
	}
}