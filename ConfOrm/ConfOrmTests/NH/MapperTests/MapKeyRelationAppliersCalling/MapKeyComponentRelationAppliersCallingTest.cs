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

namespace ConfOrmTests.NH.MapperTests.MapKeyRelationAppliersCalling
{
	public class MapKeyComponentRelationAppliersCallingTest
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
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => (new[] { typeof(Person), typeof(Skill) }).Any(tp => tp == t)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => (new[] { typeof(Person), typeof(Skill) }).Any(tp => tp == t)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(ToySkill)))).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(ToySkill)), It.Is<Type>(t => t == typeof(Skill)))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ForClass<Person>.Property(p => p.Skills)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenRegisterApplierOnPropertyThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			mapper.AddPropertyPattern(mi => mi.Name == "Level", pm => pm.Column("myLevelColumn"));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var rc = mapping.RootClasses.Single();
			var map = rc.Properties.OfType<HbmMap>().Single();
			var hbmCompositeMapKey = (HbmCompositeMapKey)map.Item;
			var hbmKeyProperty = (HbmKeyProperty)hbmCompositeMapKey.Properties.Single(p => p.Name == "Level");

			hbmKeyProperty.Columns.Single().name.Should().Be("myLevelColumn");
		}

		[Test]
		public void WhenRegisterApplierOnManyToOneThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			mapper.AddManyToOnePattern(mi=> mi.GetPropertyOrFieldType() == typeof(Skill), mtom=> mtom.Column("SkillId"));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var rc = mapping.RootClasses.Single();
			var map = rc.Properties.OfType<HbmMap>().Single();
			var hbmCompositeMapKey = (HbmCompositeMapKey)map.Item;
			var hbmKeyManyToOne = (HbmKeyManyToOne)hbmCompositeMapKey.Properties.Single(p => p.Name == "Skill");

			hbmKeyManyToOne.Columns.Single().name.Should().Be("SkillId");
		}
	}
}