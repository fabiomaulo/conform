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


namespace ConfOrmTests.NH.MapperTests
{
	public class ComponentPropertyAccessorTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			private ComponentLevel0 componentLevel0;

			public ComponentLevel0 ComponentLevel0
			{
				get { return componentLevel0; }
			}

			public IEnumerable<ComponentLevel0> Components { get; set; }
		}

		private class ComponentLevel0
		{
			private ComponentLevel1 componentLevel1;
			public string Something { get; set; }

			public ComponentLevel1 ComponentLevel1
			{
				get { return componentLevel1; }
			}
		}

		private class ComponentLevel1
		{
			public string SomethingElse { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(mc=> mc.Components)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel0)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel1)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		[Test]
		public void WhenComponentDoesNotHaveSetterThenMapToFieldByDefault()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var relation = rc.Properties.First(p => p.Name == "ComponentLevel0");
			relation.Should().Be.OfType<HbmComponent>();
			var component = (HbmComponent)relation;
			component.access.Should().Contain("nosetter");
		}

		[Test]
		public void WhenNestedCompositeElementDoesNotHaveSetterThenMapToFieldByDefault()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var collection = (HbmBag) rc.Properties.First(p => p.Name == "Components");
			var relation = (HbmCompositeElement)collection.ElementRelationship;
			relation.Should().Be.OfType<HbmCompositeElement>();
			var component = (HbmNestedCompositeElement)relation.Properties.First(p => p.Name == "ComponentLevel1");
			component.access.Should().Contain("nosetter");
		}
	}
}