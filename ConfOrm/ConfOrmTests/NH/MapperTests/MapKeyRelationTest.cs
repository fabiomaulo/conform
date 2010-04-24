using System;
using System.Collections.Generic;
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
	public class MapKeyRelationTest
	{
		private class Entity
		{
			public Guid Id { get; set; }
		}
		private class Person : Entity
		{
			public string Email { get; set; }
			public IDictionary<ToySkill, double> Skills { get; set; }
		}

		private class ToySkill
		{
			public Skill Skill { get; set; }
			public int Level { get; set; }
		}

		private class Skill : Entity
		{
			public string Name { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => (new[] { typeof(Person), typeof(Skill) }).Any(tp=> tp==t)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => (new[] { typeof(Person), typeof(Skill) }).Any(tp => tp == t)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t=> t == typeof(ToySkill)))).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(ToySkill)), It.Is<Type>(t => t == typeof(Skill)))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ForClass<Person>.Property(p=> p.Skills)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] {typeof (Person)});
		}

		[Test]
		public void WhenUseAComponentAsKeyThenMapItUsingCompositeMapKey()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var map = rc.Properties.OfType<HbmMap>().Single();
			map.Item.Should().Be.OfType<HbmCompositeMapKey>().And.ValueOf.Class.Should().Contain("ToySkill");
		}

		[Test, Ignore("Not fixed yet")]
		public void WhenUseAComponentAsKeyWithManyToOneThenUseKeyManyToOne()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var mapKey = (HbmCompositeMapKey)rc.Properties.OfType<HbmMap>().Single().Item;

			mapKey.Properties.OfType<HbmKeyManyToOne>().Should().Not.Be.Empty();
			var keyManyToOne = mapKey.Properties.OfType<HbmKeyManyToOne>().Single();
			keyManyToOne.Name.Should().Be("Skill");
			keyManyToOne.Class.Should().Contain("Skill");
		}

		[Test, Ignore("Not fixed yet")]
		public void WhenUseAComponentAsKeyWithPropertyThenUseKeyProperty()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var mapKey = (HbmCompositeMapKey)rc.Properties.OfType<HbmMap>().Single().Item;

			mapKey.Properties.OfType<HbmKeyProperty>().Should().Not.Be.Empty();
			var keyProperty = mapKey.Properties.OfType<HbmKeyProperty>().Single();
			keyProperty.Name.Should().Be("Level");
		}
	}
}