using System;
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
	public class UnidirectionalOneToOneTest
	{
		private class Customer
		{
			public int Id { get; set; }
			public Address Address { get; set; }
		}

		private class Address
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenIsManyToOneAndMasterOneToOneThenApplyUniqueAndCascadeOnAssociationProperty()
		{
			Mock<IDomainInspector> orm = GetOrmMockCustomerToAddress();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Customer) });

			HbmClass customer = mappings.RootClasses.Single();
			HbmManyToOne customerAddress = customer.Properties.OfType<HbmManyToOne>().Single();

			customerAddress.unique.Should().Be.True();
			customerAddress.cascade.Should().Be("all");
		}

		private Mock<IDomainInspector> GetOrmMockCustomerToAddress()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			orm.Setup(m => m.IsMasterOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			return orm;
		}
	}
}