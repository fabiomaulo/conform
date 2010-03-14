using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class UnidirectionalOneToOneIntegrationTest
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
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(new[] { typeof(Customer), typeof(Address) });
			orm.OneToOne<Customer, Address>();

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] {typeof (Customer)});

			HbmClass customer = mappings.RootClasses.Single();
			HbmManyToOne customerAddress = customer.Properties.OfType<HbmManyToOne>().Single();

			customerAddress.unique.Should().Be.True();
			customerAddress.cascade.Should().Be("all");
		}
	}
}