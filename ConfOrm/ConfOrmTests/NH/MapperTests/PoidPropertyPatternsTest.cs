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
	public class PoidPropertyPatternsTest
	{
		private class BaseEntity
		{
			private int id;

			public int Id
			{
				get { return id; }
			}
		}

		private class Entity: BaseEntity
		{
			
		}

		private class EntityWithoutPoidInModel
		{

		}
		
		[Test]
		public void WhenIdOnBaseEntityWithoutSetterThenAccessNoSetter()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			var mapper = new Mapper(orm.Object);

			var mapping = mapper.CompileMappingFor(new[] { typeof(Entity) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.access.Should().Be("nosetter.camelcase");
		}

		[Test, Ignore("Not fixed yet.")]
		public void WhenEntityWithoutPoidInModelThenSetTypeAndDefaultGenerator()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			var mapper = new Mapper(orm.Object);

			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityWithoutPoidInModel) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.access.Should().Be.Null();
			rc.Id.Type.Should().Not.Be.Null();
			rc.Id.generator.Should().Not.Be.Null();
		}
	}
}