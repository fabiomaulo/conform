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
	public class HierarchyTablePerConcreteClassTest
	{
		private abstract class AbstractEntity
		{
			public int Id { get; set; }
		}
		private class EntitySimple : AbstractEntity
		{
			public string Name { get; set; }
		}

		private class Hinherited : EntitySimple
		{
			public string Surname { get; set; }
		}

		[Test]
		public void MappingContainsBaseClass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyEntitySimpleMapping(mapping);
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			Mapper mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Hinherited), typeof(AbstractEntity), typeof(EntitySimple) });
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(AbstractEntity)))).Returns(true);
			orm.Setup(m => m.IsTablePerConcreteClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void MappingContainsHinheritedClass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyHinheritedMapping(mapping);
		}

		private void VerifyHinheritedMapping(HbmMapping mapping)
		{
			mapping.UnionSubclasses.Should().Have.Count.EqualTo(2);
			HbmUnionSubclass sc1 = mapping.UnionSubclasses.First(us => us.Name.Contains("EntitySimple"));
			sc1.extends.Should().Not.Be.Null().And.Contain("AbstractEntity");
			sc1.Properties.Should().Have.Count.EqualTo(1);
			sc1.Properties.Single().Name.Should().Be.EqualTo("Name");
			HbmUnionSubclass sc2 = mapping.UnionSubclasses.First(us => us.Name.Contains("Hinherited"));
			sc2.extends.Should().Not.Be.Null().And.Contain("EntitySimple");
			sc2.Properties.Should().Have.Count.EqualTo(1);
			sc2.Properties.Single().Name.Should().Be.EqualTo("Surname");
		}

		private void VerifyEntitySimpleMapping(HbmMapping mapping)
		{
			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.Should().Not.Be.Null();
			rc.Properties.Should().Have.Count.EqualTo(0);
			rc.discriminator.Should().Be.Null();
			rc.IsAbstract.Should().Be.EqualTo(true);
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerConcreteClass<AbstractEntity>();
			HbmMapping mapping = GetMapping(orm);
			VerifyEntitySimpleMapping(mapping);

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.Should("The ORM should assign a default generator").Not.Be.Null();

			VerifyHinheritedMapping(mapping);
		}

	}
}