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
	public class DefaultAssemblyAndNamespaceTest
	{
		private class MyEntity
		{
			public int Id { get; set; }
		}
		private class Inherited: MyEntity
		{
		}
		private class AEntity
		{
			public int Id { get; set; }
		}

		[Test]
		public void MapDocShouldSetDefaultAssemblyWhenTypesComesFromSameAssembly()
		{
			var di = new Mock<IDomainInspector>();
			var mapper = new Mapper(di.Object);
			
			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(AEntity) });
			mappings.assembly.Should().Be.EqualTo(typeof (MyEntity).Assembly.GetName().Name);
		}

		[Test]
		public void MapDocShouldNotSetDefaultAssemblyWhenTypesComesFromVariousAssemblies()
		{
			var di = new Mock<IDomainInspector>();
			var mapper = new Mapper(di.Object);

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(Assert) });
			mappings.assembly.Should().Be.Null();
		}

		[Test]
		public void MapDocShouldSetDefaultNamespaceWhenTypesComesFromSameNamespace()
		{
			var di = new Mock<IDomainInspector>();
			var mapper = new Mapper(di.Object);

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(AEntity) });
			mappings.@namespace.Should().Be.EqualTo(typeof(MyEntity).Namespace);
		}

		[Test]
		public void MapDocShouldNotSetDefaultNamespaceWhenTypesComesFromVariousNamespaces()
		{
			var di = new Mock<IDomainInspector>();
			var mapper = new Mapper(di.Object);

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(Assert) });
			mappings.@namespace.Should().Be.Null();
		}

		[Test]
		public void EachMapDocShouldSetDefaultAssembly()
		{
			var entities = new[] {typeof (MyEntity), typeof (AEntity), typeof(Inherited)};
			var di = new Mock<IDomainInspector>();
			di.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			di.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(Inherited)))).Returns(true);
			di.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			di.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			di.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(di.Object);
			var mappings = mapper.CompileMappingForEach(entities);

			mappings.All(md => md.Satisfy(mapDoc => typeof (MyEntity).Assembly.GetName().Name.Equals(mapDoc.assembly)));
		}

		[Test]
		public void EachMapDocShouldSetDefaultNamespace()
		{
			var entities = new[] { typeof(MyEntity), typeof(AEntity), typeof(Inherited) };
			var di = new Mock<IDomainInspector>();
			di.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			di.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(Inherited)))).Returns(true);
			di.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			di.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			di.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(di.Object);
			var mappings = mapper.CompileMappingForEach(entities);

			mappings.All(md => md.Satisfy(mapDoc => typeof(MyEntity).Namespace.Equals(mapDoc.@namespace)));
		}
	}
}