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
	public class ManyToManyApplyCascade
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<Animal> Pets { get; set; }
		}

		private class Animal
		{
			public int Id { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.ApplyCascade(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Animal)))).Returns(Cascade.Persist);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Pets")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person), typeof(Animal) });
		}

		[Test]
		public void MappingThroughMock()
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
			var relation = rc.Properties.First(p => p.Name == "Pets");
			relation.Should().Be.OfType<HbmBag>();
			var collection = (HbmBag)relation;
			collection.cascade.Should().Contain("save-update");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.TablePerClass<Animal>();
			orm.ManyToMany<Person, Animal>();
			orm.Cascade<Person, Animal>(Cascade.Persist);
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}

	}
}