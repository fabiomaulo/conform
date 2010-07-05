using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class ManyToManyCustomizersInvocationTest
	{
		private class Animal
		{
			public int Id { get; set; }
		}

		private class Person
		{
			public int Id { get; set; }
			public ICollection<Animal> Pets { get; set; }
			public IDictionary<string, Animal> Farm { get; set; }
		}

		private Mock<IDomainInspector> GetBaseMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof (Person)), It.Is<Type>(t => t == typeof (Animal)))).
				Returns(true);

			return orm;
		}

		[Test]
		public void WhenRegisteredCustomizerForBagThenInvokeElementMapperAction()
		{
			Mock<IDomainInspector> orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);
			bool customizerInvoked = false;
			var propertyPath = new PropertyPath(null, ForClass<Person>.Property(p => p.Pets));
			var customizersHolder = new CustomizersHolder();
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => customizerInvoked = true);

			var mapper = new Mapper(orm.Object, customizersHolder);
			mapper.CompileMappingFor(new[] {typeof (Person)});

			customizerInvoked.Should().Be.True();
		}

		[Test]
		public void WhenRegisteredCustomizerForDictionaryThenCallElementMapperAction()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsDictionary(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Farm)))).Returns(true);
			bool customizerInvoked = false;
			var propertyPath = new PropertyPath(null, ForClass<Person>.Property(p => p.Farm));
			var customizersHolder = new CustomizersHolder();
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => customizerInvoked = true);

			var mapper = new Mapper(orm.Object, customizersHolder);
			mapper.CompileMappingFor(new[] { typeof(Person) });

			customizerInvoked.Should().Be.True();
		}

	}
}