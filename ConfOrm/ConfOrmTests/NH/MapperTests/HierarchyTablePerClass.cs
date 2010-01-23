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
	public class HierarchyTablePerClass
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

		private HbmMapping GetMapping(IDomainInspector domainInspector) {
			Mapper mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Hinherited), typeof(EntitySimple) });
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof (EntitySimple)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
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
			mapping.JoinedSubclasses.Should().Have.Count.EqualTo(1);
			HbmJoinedSubclass jsc = mapping.JoinedSubclasses.Single();
			jsc.extends.Should().Not.Be.Null().And.Contain("EntitySimple");
			jsc.key.Should().Not.Be.Null();
			jsc.key.Columns.Should().Not.Be.Empty();
			jsc.key.Columns.Single().name.Should().Not.Be.Null().And.Not.Be.Empty();
			jsc.Properties.Should().Have.Count.EqualTo(1);
			jsc.Properties.Single().Name.Should().Be.EqualTo("Surname");
		}

		private void VerifyEntitySimpleMapping(HbmMapping mapping)
		{
			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.Should().Not.Be.Null();
			rc.Id.generator.Should().Not.Be.Null();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.First().Name.Should().Be.EqualTo("Name");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<EntitySimple>();
			HbmMapping mapping = GetMapping(orm);
			VerifyEntitySimpleMapping(mapping);
			VerifyHinheritedMapping(mapping);
		}
	}
}