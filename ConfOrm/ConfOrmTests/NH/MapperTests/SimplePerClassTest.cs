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
	public class SimplePerClassTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void MappingContainsClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi=> mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntitySimple) });

			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			var rc = mapping.RootClasses.Single();
			rc.Id.Should().Not.Be.Null();
			rc.Id.generator.Should().Not.Be.Null();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.First().Name.Should().Be.EqualTo("Name");
		}
	}
}