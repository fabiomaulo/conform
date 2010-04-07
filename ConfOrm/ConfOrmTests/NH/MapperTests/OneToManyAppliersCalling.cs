using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class OneToManyAppliersCalling
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<Animal> Pets { get; set; }
		}

		private class Animal
		{

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
		public void WhenRegisteredApplierForBagThenCallApplyForMemberInfo()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var oneToManyApplier = new Mock<IPatternApplier<MemberInfo, IOneToManyMapper>>();
			oneToManyApplier.Setup(x => x.Match(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.Pets))))).Returns(true);

			mapper.PatternsAppliers.OneToMany.Add(oneToManyApplier.Object);

			mapper.CompileMappingFor(new[] { typeof(Person) });

			oneToManyApplier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.Pets))), It.IsAny<IOneToManyMapper>()));
		}

		[Test]
		public void WhenRegisteredApplierForBagThenCallApplyForPropertyPath()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.Pets)))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var oneToManyApplier = new Mock<IPatternApplier<PropertyPath, IOneToManyMapper>>();
			oneToManyApplier.Setup(x => x.Match(It.Is<PropertyPath>(pp => pp.Equals(new PropertyPath(null, ForClass<Person>.Property(p => p.Pets)))))).Returns(true);

			mapper.PatternsAppliers.OneToManyPath.Add(oneToManyApplier.Object);

			mapper.CompileMappingFor(new[] { typeof(Person) });

			oneToManyApplier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.Equals(new PropertyPath(null, ForClass<Person>.Property(p => p.Pets)))), It.IsAny<IOneToManyMapper>()));
		}
	}
}