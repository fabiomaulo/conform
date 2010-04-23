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
	public class ElementsCustomizersCallingTest
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
			var called = false;
			var mapper = new Mapper(orm.Object);
			mapper.Class<Person>(cm=> cm.Bag(person=> person.PetsNames, cpm=> { }, cerm=> cerm.Element(em=> called = true)));

			mapper.CompileMappingFor(new[] { typeof(Person) });

			called.Should().Be.True();
		}
	}
}