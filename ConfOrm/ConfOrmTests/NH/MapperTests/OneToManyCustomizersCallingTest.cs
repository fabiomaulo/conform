using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class OneToManyCustomizersCallingTest
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<Animal> Pets { get; set; }
			public IDictionary<string, Animal> Farm { get; set; }
		}

		private class Animal
		{
			public int Id { get; set; }			
		}

		private Mock<IDomainInspector> GetBaseMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Animal)))).Returns(true);

			return orm;
		}

		[Test]
		public void WhenRegisteredCustomizerForBagThenCallElementMapperAction()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);
			var called = false;
			var mapper = new Mapper(orm.Object);
			mapper.Class<Person>(cm => cm.Bag(person => person.Pets, cpm => { }, cerm => cerm.OneToMany(em => called = true)));

			mapper.CompileMappingFor(new[] { typeof(Person) });

			called.Should().Be.True();
		}
		[Test]
		public void WhenRegisteredCustomizerForSetThenCallElementMapperAction()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsSet(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);
			var called = false;
			var mapper = new Mapper(orm.Object);
			mapper.Class<Person>(cm => cm.Set(person => person.Pets, cpm => { }, cerm => cerm.OneToMany(em => called = true)));

			mapper.CompileMappingFor(new[] { typeof(Person) });

			called.Should().Be.True();
		}

		[Test]
		public void WhenRegisteredCustomizerForListThenCallElementMapperAction()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsList(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);
			var called = false;
			var mapper = new Mapper(orm.Object);
			mapper.Class<Person>(cm => cm.List(person => person.Pets, cpm => { }, cerm => cerm.OneToMany(em => called = true)));

			mapper.CompileMappingFor(new[] { typeof(Person) });

			called.Should().Be.True();
		}

		[Test]
		public void WhenRegisteredCustomizerForDictionaryThenCallElementMapperAction()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsDictionary(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Farm)))).Returns(true);
			var called = false;
			var mapper = new Mapper(orm.Object);
			mapper.Class<Person>(
				cm => cm.Map(person => person.Farm, cpm => { }, mkrm => { }, cerm => cerm.OneToMany(em => called = true)));

			mapper.CompileMappingFor(new[] { typeof(Person) });

			called.Should().Be.True();
		}
	}
}