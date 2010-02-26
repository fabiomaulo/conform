using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class JoinedSubclassCustomizerCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		private class Inherited : MyClass
		{
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => typeof(MyClass) == t))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void CustomizerCalledPerSubclass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool isCalled = false;

			mapper.JoinedSubclass<Inherited>(ca =>
			{
				ca.Should().Not.Be.Null();
				isCalled = true;
			});

			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			isCalled.Should().Be(true);
		}

		[Test]
		public void InvokeCustomizers()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			Mock<ICustomizersHolder> customizers = new Mock<ICustomizersHolder>();
			var mapper = new Mapper(orm.Object, customizers.Object);
			mapper.JoinedSubclass<Inherited>(ca =>{ });

			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			customizers.Verify(
				c => c.InvokeCustomizers(It.Is<Type>(t => t == typeof (Inherited)), It.IsAny<IJoinedSubclassAttributesMapper>()));
		}

		[Test]
		public void SetTableSpecifications()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var mapper = new Mapper(orm.Object);
			mapper.JoinedSubclass<Inherited>(x =>
			{
				x.Table("tabella");
				x.Schema("dbo");
				x.Catalog("catalogo");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.JoinedSubclasses.Single();
			hbmClass.table.Should().Be("tabella");
			hbmClass.catalog.Should().Be("catalogo");
			hbmClass.schema.Should().Be("dbo");
		}
	}
}