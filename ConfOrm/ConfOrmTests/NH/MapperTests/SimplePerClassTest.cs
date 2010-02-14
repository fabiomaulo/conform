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
	public class SimplePerClassTest
	{
		[Test]
		public void MappingContainsClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			VerifySimpleEntity(orm.Object);
		}

		private void VerifySimpleEntity(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			HbmMapping mapping = mapper.CompileMappingFor(new[] {typeof (EntitySimple)});

			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.Should().Not.Be.Null();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.First().Name.Should().Be.EqualTo("Name");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<EntitySimple>();
			VerifySimpleEntity(orm);
		}

		#region Nested type: EntitySimple

		private class EntitySimple
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		#endregion
	}
}