using System;
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
	public class ComponentAttributesCustomizationByPropertyPath
	{
		private class MyClass
		{
			public int Id { get; set; }

			public ComponentLevel0 ComponentLevel00 { get; set; }
			public ComponentLevel0 ComponentLevel01 { get; set; }
		}

		private class ComponentLevel0
		{
			public string Something { get; set; }

			public ComponentLevel1 ComponentLevel1 { get; set; }
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
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel0)))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ComponentLevel1)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenCustomizePerPropertyThenEachPropertyInDifferentWays()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Customize<MyClass>(cm =>
			                          	{
			                          		cm.Component(myclass => myclass.ComponentLevel00, compo => compo.Lazy(true));
																		cm.Component(myclass => myclass.ComponentLevel01, compo => compo.Lazy(false));
																	});

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var relation0 = (HbmComponent) rc.Properties.First(p => p.Name == "ComponentLevel00");
			relation0.IsLazyProperty.Should().Be.True();
			var relation1 = (HbmComponent)rc.Properties.First(p => p.Name == "ComponentLevel01");
			relation1.IsLazyProperty.Should().Be.False();
		}

		[Test]
		public void WhenCustomizeNestedComponetPerPropertyThenEachPropertyInDifferentWays()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);

			mapper.Class<MyClass>(cm =>
			{
				cm.Component(myclass => myclass.ComponentLevel00, compo00 => compo00.Component(c => c.ComponentLevel1, compo00_1 => compo00_1.Lazy(true)));
				cm.Component(myclass => myclass.ComponentLevel01, compo01 => compo01.Component(c => c.ComponentLevel1, compo00_1 => compo00_1.Lazy(false)));
			});

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var relation0 = ((HbmComponent)rc.Properties.First(p => p.Name == "ComponentLevel00")).Properties.First(p => p.Name == "ComponentLevel1");
			relation0.IsLazyProperty.Should().Be.True();
			var relation1 = ((HbmComponent)rc.Properties.First(p => p.Name == "ComponentLevel01")).Properties.First(p => p.Name == "ComponentLevel1");
			relation1.IsLazyProperty.Should().Be.False();
		}
	}
}