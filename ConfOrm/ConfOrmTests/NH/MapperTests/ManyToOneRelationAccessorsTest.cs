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
	public class ManyToOneRelationAccessorsTest
	{
		private class AEntity
		{
			public int Id { get; set; }
			private BEntity noSetterB;
			public BEntity NoSetterB
			{
				get { return noSetterB; }
			}

			public BEntity ReadOnlyB
			{
				get { return null; }
			}
			private BEntity fieldB;

			public object FieldB
			{
				get { return fieldB; }
				set { fieldB = (BEntity)value; }
			}
		}

		private class BEntity
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
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(AEntity)), It.Is<Type>(t => t == typeof(BEntity)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(AEntity) });
		}

		[Test]
		public void MappingApplyAccessors()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			HbmClass rc = mapping.RootClasses.Single();
			var nosetterB = rc.Properties.First(p => p.Name == "NoSetterB");
			var fieldB = rc.Properties.First(p => p.Name == "FieldB");
			var readonlyB = rc.Properties.First(p => p.Name == "ReadOnlyB");
			nosetterB.Access.Should().Contain("nosetter");
			fieldB.Access.Should().Contain("field");
			readonlyB.Access.Should().Be("readonly");
		}
	}
}