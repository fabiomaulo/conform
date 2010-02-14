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
	public class HierarchyTablePerClassHierarchyTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
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
			return mapper.CompileMappingFor(new[] { typeof(Hinherited), typeof(EntitySimple) });
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(EntitySimple)))).Returns(true);
			orm.Setup(m => m.IsTablePerClassHierarchy(It.IsAny<Type>())).Returns(true);
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
			mapping.SubClasses.Should().Have.Count.EqualTo(1);
			HbmSubclass sc = mapping.SubClasses.Single();
			sc.extends.Should().Not.Be.Null().And.Contain("EntitySimple");
			sc.Properties.Should().Have.Count.EqualTo(1);
			sc.Properties.Single().Name.Should().Be.EqualTo("Surname");
		}

		private void VerifyEntitySimpleMapping(HbmMapping mapping)
		{
			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.Should().Not.Be.Null();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.First().Name.Should().Be.EqualTo("Name");
			rc.discriminator.Should().Not.Be.Null();
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClassHierarchy<EntitySimple>();
			HbmMapping mapping = GetMapping(orm);
			VerifyEntitySimpleMapping(mapping);

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.Should("The ORM should assign a default generator").Not.Be.Null();

			VerifyHinheritedMapping(mapping);
		}
	}
}