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
	public class BidirectionalOneToOnePrimaryKeyAssociationTest
	{
		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private class Customer : BaseEntity
		{
			public Address Address { get; set; }
		}

		private class Address : BaseEntity
		{
			public Customer Customer { get; set; }
		}

		[Test]
		public void WhenCustomerIsTheMasterThenApplyCascadeOnAddressProperty()
		{
			Mock<IDomainInspector> orm = GetOrmMockCustomerToAddress();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Customer) });

			HbmClass customer = mappings.RootClasses.Single();
			HbmOneToOne customerAddress = customer.Properties.OfType<HbmOneToOne>().Single();

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
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).
				Returns(true);
			orm.Setup(m => m.IsMasterOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			return orm;
		}

		[Test]
		public void WhenCustomerIsTheMasterThenApplyForeingGeneratorAndConstrainedOnCustomerOfAddress()
		{
			Mock<IDomainInspector> orm = GetOrmMockCustomerToAddress();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Address) });

			HbmClass address = mappings.RootClasses.Single();

			address.Id.generator.Should().Not.Be.Null();
			address.Id.generator.@class.Should().Be("foreign");
			address.Id.generator.param.Should().Have.Count.EqualTo(1);
			var hbmParam = address.Id.generator.param.Single();
			hbmParam.name.Should().Be("property");
			hbmParam.GetText().Should().Be("Customer");

			HbmOneToOne addressCustomer = address.Properties.OfType<HbmOneToOne>().Single();
			addressCustomer.constrained.Should().Be.True();
		}

		[Test]
		public void WhenAddressIsTheMasterThenApplyCascadeOnCustomerProperty()
		{
			Mock<IDomainInspector> orm = GetOrmMockAddressToCustomer();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Address) });

			HbmClass address = mappings.RootClasses.Single();
			HbmOneToOne addressCustomer = address.Properties.OfType<HbmOneToOne>().Single();

			addressCustomer.cascade.Should().Be("all");
		}

		private Mock<IDomainInspector> GetOrmMockAddressToCustomer()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).
				Returns(true);
			orm.Setup(m => m.IsMasterOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).
				Returns(true);
			return orm;
		}

		[Test]
		public void WhenAddressIsTheMasterThenApplyForeingGeneratorAndConstrainedOnAddressOfCustomer()
		{
			Mock<IDomainInspector> orm = GetOrmMockAddressToCustomer();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Customer) });

			HbmClass address = mappings.RootClasses.Single();

			address.Id.generator.Should().Not.Be.Null();
			address.Id.generator.@class.Should().Be("foreign");
			address.Id.generator.param.Should().Have.Count.EqualTo(1);
			var hbmParam = address.Id.generator.param.Single();
			hbmParam.name.Should().Be("property");
			hbmParam.GetText().Should().Be("Address");

			HbmOneToOne customerAddress = address.Properties.OfType<HbmOneToOne>().Single();
			customerAddress.constrained.Should().Be.True();
		}
	}
}