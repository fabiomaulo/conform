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
	public class HierarchyTablePerClassJumpingTest
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

		private class Hinherited2 : Hinherited
		{
			public string Surname2 { get; set; }
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			Mapper mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Hinherited), typeof(Hinherited2), typeof(EntitySimple) });
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t != typeof(Hinherited)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(EntitySimple)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void MappingsShouldContainOnlyASubclass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);
			mapping.JoinedSubclasses.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void TheBaseClassShouldNotBeTheNotEntity()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);
			var hbmclass = mapping.JoinedSubclasses.Single();
			hbmclass.extends.Should().Contain("EntitySimple");
		}

		[Test]
		public void PropertiesShouldContainPropertiesOfJumpedClass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);
			var hbmclass = mapping.JoinedSubclasses.Single();
			hbmclass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Surname2", "Surname");
		}
	}
}