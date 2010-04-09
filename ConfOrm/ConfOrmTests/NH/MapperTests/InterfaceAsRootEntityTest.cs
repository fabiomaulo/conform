using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class InterfaceAsRootEntityTest
	{
		private interface IEntity<T>
		{
			T Id { get; }
		}

		private interface IVersionedEntity: IEntity<int>
		{
			int Version { get; }
		}

		private interface IMyEntity : IVersionedEntity
		{
			
		}

		public Mock<IDomainInspector> GetBaseMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t=> typeof(IMyEntity)== t))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t=> typeof(IMyEntity)== t))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsVersion(It.Is<MemberInfo>(mi => mi.Name == "Version"))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenRootClassIsInterfaceThenFindId()
		{
			var orm = GetBaseMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] {typeof(IMyEntity)});
			var rc = mappings.RootClasses.Single();
			rc.Id.name.Should().Be("Id");
			rc.Id.Type.name.Should().Contain("Int32");
		}

		[Test]
		public void WhenRootClassIsInterfaceThenFindVersion()
		{
			var orm = GetBaseMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(IMyEntity) });
			var rc = mappings.RootClasses.Single();
			rc.Version.Should().Not.Be.Null();
			rc.Version.name.Should().Be("Version");
		}
	}
}