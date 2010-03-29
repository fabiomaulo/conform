using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class ManyToManyApplierCalling
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
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Animal)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Pets")))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenRegisteredApplierThenCallApply()
		{
			var orm = GetMockedDomainInspector();

			var mapper = new Mapper(orm.Object);
			var manyToManyApplier = new Mock<IPatternApplier<MemberInfo, IManyToManyMapper>>();
			manyToManyApplier.Setup(x => x.Match(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.Pets))))).Returns(true);
			mapper.PatternsAppliers.ManyToMany.Add(manyToManyApplier.Object);
			mapper.CompileMappingFor(new[] { typeof(Person), typeof(Animal) });

			manyToManyApplier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.Pets))), It.IsAny<IManyToManyMapper>()));
		}
	}
}