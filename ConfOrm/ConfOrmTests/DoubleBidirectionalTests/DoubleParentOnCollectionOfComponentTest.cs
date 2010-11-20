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

namespace ConfOrmTests.DoubleBidirectionalTests
{
	public class DoubleParentOnCollectionOfComponentTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public IEnumerable<Component> Components { get; set; }
		}

		private class Component
		{
			public MyClass Rel1 { get; set; }
			public MyClass Rel2 { get; set; }
			public MyClass Rel3 { get; set; }
			private MyClass owner;
			public MyClass Owner
			{
				get { return owner; }
			}

			public string First { get; set; }
			public string Last { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t != typeof(Component)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(Component)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi.Equals(ForClass<MyClass>.Property(x => x.Components))))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Component)))).Returns(true);
			orm.Setup(m => m.GetBidirectionalMember(typeof(MyClass), It.Is<MemberInfo>(mi => mi.Equals(ForClass<MyClass>.Property(x => x.Components))), typeof(Component))).Returns(ForClass<Component>.Property(x => x.Owner));
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		[Test]
		public void MappingThroughMock()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMappingContainsClassWithComponentAndParent(mapping);
		}

		private void VerifyMappingContainsClassWithComponentAndParent(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyClass"));
			var relation = (HbmBag)rc.Properties.First(p => p.Name == "Components");
			var component = (HbmCompositeElement)relation.ElementRelationship;
			component.Parent.Should().Not.Be.Null();
			component.Parent.name.Should().Be.EqualTo("Owner");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.Bidirectional<MyClass, Component>(mc => mc.Components, c => c.Owner);
			HbmMapping mapping = GetMapping(orm);

			VerifyMappingContainsClassWithComponentAndParent(mapping);
		}

		[Test]
		public void IntegrationWithObjectRelationalMapperRegisteringTheInverse()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.Bidirectional<Component, MyClass>(component => component.Owner, myClass => myClass.Components);
			HbmMapping mapping = GetMapping(orm);

			VerifyMappingContainsClassWithComponentAndParent(mapping);
		}
	}
}