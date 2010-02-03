using System;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class MappingPerClass
	{
		private class Person
		{
			public int Id { get; set; }
		}

		private class Address
		{
			public int Id { get; set; }
		}
		private class SpecialAddress : Address
		{
			public string SpecialInfo { get; set; }
		}

		[Test]
		public void CreateAMappingForEachClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Address) || t == typeof(SpecialAddress)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person) || t == typeof(Address)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			mapper.CompileMappingForEach(new[] {typeof (Person), typeof (Address)}).Should().Have.Count.EqualTo(2);
			mapper.CompileMappingForEach(new[] { typeof(Person), typeof(SpecialAddress), typeof(Address) }).Should().Have.Count.EqualTo(3);
		}
	}
}