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
	public class NaturalIdTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public Related Related { get; set; }
			public MyComponent MyComponent { get; set; }
			public object Any { get; set; }
		}

		private class Related
		{
			public int Id { get; set; }			
		}
		private class MyComponent
		{
			public string FirstName { get; set; }
		}
		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);

			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(MyComponent)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(MyClass)),It.Is<Type>(t => t == typeof(Related)))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociation(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Any")))).Returns(true);

			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			orm.Setup(m => m.IsMemberOfNaturalId(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Name")))).Returns(true);
			orm.Setup(m => m.IsMemberOfNaturalId(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Related")))).Returns(true);
			orm.Setup(m => m.IsMemberOfNaturalId(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("MyComponent")))).Returns(true);
			orm.Setup(m => m.IsMemberOfNaturalId(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Any")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		[Test]
		public void WhenFoundNaturalIdPropertyThenAddItToNaturalIdInsteadClass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.Should().Have.Count.EqualTo(0);
			rc.naturalid.Should().Not.Be.Null();
			var hbmNId = rc.naturalid;
			hbmNId.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Name", "Related", "MyComponent", "Any");
			hbmNId.Properties.Satisfy(ps=> ps.OfType<HbmProperty>().Any());
			hbmNId.Properties.Satisfy(ps => ps.OfType<HbmManyToOne>().Any());
			hbmNId.Properties.Satisfy(ps => ps.OfType<HbmComponent>().Any());
			hbmNId.Properties.Satisfy(ps => ps.OfType<HbmAny>().Any());
		}

		[Test]
		public void WhenNotSupportedElementThenThrow()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(Related)))).Returns(true);

			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(Related)))).Returns(true);

			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			orm.Setup(m => m.IsMemberOfNaturalId(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Related")))).Returns(true);
			
			var mapper = new Mapper(orm.Object);
			Executing.This(() => mapper.CompileMappingFor(new[] {typeof (MyClass)})).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanCustomize()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			mapper.Class<MyClass>(cm => cm.NaturalId(nidm => nidm.Mutable(true)));
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.naturalid.mutable.Should().Be.True();
		}
	}
}