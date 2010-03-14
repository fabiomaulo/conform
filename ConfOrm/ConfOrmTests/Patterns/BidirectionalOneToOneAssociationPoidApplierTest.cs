using System;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToOneAssociationPoidApplierTest
	{
		public class Customer
		{
			public int Id { get; set; }
			public Address Address { get; set; }
		}

		public class Address
		{
			public int Id { get; set; }
			public Customer Customer { get; set; }
		}

		[Test]
		public void WhenPrimaryKeyAssociationThenShouldMatch()
		{
			var domainInspector = new Mock<IDomainInspector>();
			domainInspector.Setup(di => di.IsEntity(It.IsAny<Type>())).Returns(true);
			domainInspector.Setup(
				di => di.IsOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).Returns(true);
			domainInspector.Setup(
				di => di.IsOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).Returns(true);
			domainInspector.Setup(
				di => di.IsMasterOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).Returns(true);
			var pattern = new BidirectionalOneToOneAssociationPoidApplier(domainInspector.Object);

			pattern.Match(typeof(Customer).GetProperty("Id")).Should().Be.False();
			pattern.Match(typeof(Address).GetProperty("Id")).Should().Be.True();
		}

		[Test]
		public void WhenForeignKeyAssociationThenShouldntMatch()
		{
			var domainInspector = new Mock<IDomainInspector>();
			domainInspector.Setup(di => di.IsEntity(It.IsAny<Type>())).Returns(true);
			domainInspector.Setup(
				di => di.IsManyToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).Returns(true);
			domainInspector.Setup(
				di => di.IsOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).Returns(true);
			domainInspector.Setup(
				di => di.IsOneToOne(It.Is<Type>(t => t == typeof(Customer)), It.Is<Type>(t => t == typeof(Address)))).Returns(true);
			domainInspector.Setup(
				di => di.IsMasterOneToOne(It.Is<Type>(t => t == typeof(Address)), It.Is<Type>(t => t == typeof(Customer)))).Returns(true);
			var pattern = new BidirectionalOneToOneAssociationPoidApplier(domainInspector.Object);

			pattern.Match(typeof(Customer).GetProperty("Id")).Should().Be.False();
			pattern.Match(typeof(Address).GetProperty("Id")).Should().Be.False();
		}
	}
}