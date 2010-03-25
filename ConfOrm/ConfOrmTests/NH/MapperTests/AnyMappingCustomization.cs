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
	public class AnyMappingCustomization
	{
		private class MyClass
		{
			public Guid Id { get; set; }
			public object OtherReferenceClass { get; set; }
			public MyComponent MyComponent { get; set; }
		}

		private class Subclass : MyClass
		{
			public object AnyClass { get; set; }
		}

		private class MyComponent
		{
			public object AnyClass { get; set; }
		}
		private class MyReferenceClass
		{
			public int Id { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(MyComponent)))).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociations(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("OtherReferenceClass")))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociations(It.Is<MemberInfo>(p => p == typeof(Subclass).GetProperty("AnyClass")))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociations(It.Is<MemberInfo>(p => p == typeof(MyComponent).GetProperty("AnyClass")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Subclass) });
		}

		[Test]
		public void WhenHeterogeneousAssociationInEntityThenSetIdMetaTypeToPoidTypeOfEntityByDefault()
		{
			var orm = GetMockedDomainInspector();
			var mappings = GetMapping(orm.Object);
			var hbmClass = mappings.RootClasses.Single();
			var hbmAny = (HbmAny)hbmClass.Properties.Single(p => p.Name == "OtherReferenceClass");
			hbmAny.idtype.Should().Be("Guid");
		}

		[Test]
		public void WhenHeterogeneousAssociationInSubclassThenSetIdMetaTypeToPoidTypeOfEntityByDefault()
		{
			var orm = GetMockedDomainInspector();
			var mappings = GetMapping(orm.Object);
			var hbmClass = mappings.JoinedSubclasses.Single();
			var hbmAny = (HbmAny)hbmClass.Properties.Single(p => p.Name == "AnyClass");
			hbmAny.idtype.Should().Be("Guid");
		}

		[Test]
		public void WhenHeterogeneousAssociationInComponentThenSetIdMetaTypeToPoidTypeOfEntityByDefault()
		{
			var orm = GetMockedDomainInspector();
			var mappings = GetMapping(orm.Object);
			var hbmClass = mappings.RootClasses.Single();
			var hbmComponent = hbmClass.Properties.OfType<HbmComponent>().Single();
			var hbmAny = (HbmAny)hbmComponent.Properties.Single(p => p.Name == "AnyClass");
			hbmAny.idtype.Should().Be("Guid");
		}
	}
}