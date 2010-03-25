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
	public class AnyMappingSimpleTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
			private object otherReferenceClass;
			public object OtherReferenceClass
			{
				get { return otherReferenceClass; }
			}
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociation(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("MyReferenceClass")))).Returns(true);
			orm.Setup(m => m.IsHeterogeneousAssociation(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("OtherReferenceClass")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] {typeof (MyClass)});
		}

		[Test]
		public void ContainHbmAnyElement()
		{
			var orm = GetMockedDomainInspector();
			var mappings = GetMapping(orm.Object);
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.Properties.Single(p => p.Name == "MyReferenceClass").Should().Be.OfType<HbmAny>();
		}

		[Test]
		public void ShouldCallDefaultAccessorAppliers()
		{
			var orm = GetMockedDomainInspector();
			var mappings = GetMapping(orm.Object);
			var hbmClass = mappings.RootClasses.Single();
			var hbmAny = (HbmAny) hbmClass.Properties.Single(p => p.Name == "OtherReferenceClass");
			hbmAny.Access.Should().Not.Be.Null();
		}
	}
}