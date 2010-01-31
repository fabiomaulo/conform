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
	public class ComponentPropertyTest
	{
		private class Person
		{
			public int Id { get; set; }
			public Name Name { get; set; }
		}

		private class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t=> t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t=> t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Name)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person)});
		}

		[Test]
		public void MappingContainsClassWithComponent()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMapping(mapping);
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Name");
			var relation = rc.Properties.First(p => p.Name == "Name");
			relation.Should().Be.OfType<HbmComponent>();
			var component = (HbmComponent)relation;
			component.Properties.Should().Have.Count.EqualTo(2);
			component.Properties.Select(p => p.Name).Should().Have.SameValuesAs("First", "Last");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}
	}
}