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
	public class BidirectionalOneToOneForeignKeyAssociationTest
	{
		private class Customer
		{
			public int Id { get; set; }
			public Address Address { get; set; }
		}

		private class Address
		{
			public int Id { get; set; }
			public Customer Customer { get; set; }
		}

		[Test]
		public void WhenCustomerIsTheMasterThenApplyUniqueAndCascadeOnAddressProperty()
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
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof (Customer)), It.Is<Type>(t => t == typeof (Address)))).
				Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof (Address)), It.Is<Type>(t => t == typeof (Customer)))).
				Returns(true);
			return orm;
		}

		[Test]
		public void WhenCustomerIsTheMasterThenApplyPropertyRefOnCustomerPropertyInAddress()
		{
			Mock<IDomainInspector> orm = GetOrmMockCustomerToAddress();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Address) });

			HbmClass address = mappings.RootClasses.Single();
			HbmOneToOne addressCustomer = address.Properties.OfType<HbmOneToOne>().Single();

			addressCustomer.propertyref.Should().Be("Address");
		}

		[Test]
		public void WhenAddressIsTheMasterThenApplyUniqueAndCascadeOnCustomerProperty()
		{
			Mock<IDomainInspector> orm = GetOrmMockAddressToCustomer();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Address) });

			HbmClass address = mappings.RootClasses.Single();
			HbmManyToOne addressCustomer = address.Properties.OfType<HbmManyToOne>().Single();

			addressCustomer.unique.Should().Be.True();
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
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).
				Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).
				Returns(true);
			return orm;
		}

		[Test]
		public void WhenAddressIsTheMasterThenApplyPropertyRefOnAddressPropertyInCustomer()
		{
			Mock<IDomainInspector> orm = GetOrmMockAddressToCustomer();

			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Customer) });

			HbmClass customer = mappings.RootClasses.Single();
			HbmOneToOne customerAddress = customer.Properties.OfType<HbmOneToOne>().Single();

			customerAddress.propertyref.Should().Be("Customer");
		}

		[Test]
		public void IntegrationWithOrmWhenCustomerIsTheMaster()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(new[] {typeof (Customer), typeof (Address)});
			orm.ManyToOne<Customer, Address>();
			orm.OneToOne<Address, Customer>();

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Customer), typeof(Address) });

			HbmClass customer = mappings.RootClasses.Single(c=> c.Name.Contains("Customer"));
			HbmManyToOne customerAddress = customer.Properties.OfType<HbmManyToOne>().Single();
			customerAddress.unique.Should().Be.True();
			customerAddress.cascade.Should().Be("all");

			HbmClass address = mappings.RootClasses.Single(c => c.Name.Contains("Address"));
			HbmOneToOne addressCustomer = address.Properties.OfType<HbmOneToOne>().Single();
			addressCustomer.propertyref.Should().Be("Address");
		}
	}
}