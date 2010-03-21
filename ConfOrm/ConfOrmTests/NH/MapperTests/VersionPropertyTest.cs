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
	public class VersionPropertyTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int MyVersion { get; set; }
			public string AProperty { get; set; }
		}

		[Test]
		public void WhenNoVersionDefinedThenUseNormalProperty()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.Should().Have.Count.EqualTo(2);
			rc.Properties.Select(p => p.Name).Should().Have.SameValuesAs("MyVersion", "AProperty");
		}

		[Test, Ignore("Not fixed yet.")]
		public void WhenVersionDefinedThenVersionProperty()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsVersion(It.Is<MemberInfo>(mi => mi.Name == "MyVersion"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Version.Should().Not.Be.Null();
			rc.Version.name.Should().Be("MyVersion");
		}

		[Test]
		public void WhenVersionDefinedThenExcludesFromProperties()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsVersion(It.Is<MemberInfo>(mi => mi.Name == "MyVersion"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.Single().Name.Should().Be("AProperty");
		}
	}
}