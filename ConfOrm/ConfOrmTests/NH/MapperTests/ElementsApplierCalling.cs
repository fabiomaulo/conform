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
	public class ElementsApplierCalling
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<string> PetsNames { get; set; }
		}

		private Mock<IDomainInspector> GetBaseMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			return orm;
		}

		[Test]
		public void WhenRegisteredApplierForBagThenCallApplyForMemberInfo()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.PetsNames)))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var elementApplier = new Mock<IPatternApplier<MemberInfo, IElementMapper>>();
			elementApplier.Setup(x => x.Match(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.PetsNames))))).Returns(true);

			mapper.PatternsAppliers.Element.Add(elementApplier.Object);

			mapper.CompileMappingFor(new[] {typeof (Person)});

			elementApplier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Person>.Property(p => p.PetsNames))), It.IsAny<IElementMapper>()));
		}

		[Test]
		public void WhenRegisteredApplierForBagThenCallApplyForPropertyPath()
		{
			var orm = GetBaseMockedDomainInspector();
			orm.Setup(x => x.IsBag(It.Is<MemberInfo>(m => m == ForClass<Person>.Property(p => p.PetsNames)))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var elementApplier = new Mock<IPatternApplier<PropertyPath, IElementMapper>>();
			elementApplier.Setup(x => x.Match(It.Is<PropertyPath>(pp => pp.Equals(new PropertyPath(null, ForClass<Person>.Property(p => p.PetsNames)))))).Returns(true);

			mapper.PatternsAppliers.ElementPath.Add(elementApplier.Object);

			mapper.CompileMappingFor(new[] { typeof(Person) });

			elementApplier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.Equals(new PropertyPath(null, ForClass<Person>.Property(p => p.PetsNames)))), It.IsAny<IElementMapper>()));
		}
	}
}