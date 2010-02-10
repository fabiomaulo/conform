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
	public class PrivatePropertiesTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
			private string Name { get; set; }
		}

		[Test]
		public void MapPrivatePropertyAsPersistentProperty()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(EntitySimple) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.First().Name.Should().Be.EqualTo("Name");
		}
	}
}