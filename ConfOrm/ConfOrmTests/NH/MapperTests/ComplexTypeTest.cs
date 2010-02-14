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
	public class ComplexTypeTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MonetaryAmount Amount { get; set; }
		}

		private class MonetaryAmount
		{
			private readonly decimal value;
			private readonly string currency;

			public MonetaryAmount(decimal value, string currency)
			{
				this.value = value;
				this.currency = currency;
			}

			public string Currency
			{
				get { return currency; }
			}

			public decimal Value
			{
				get { return value; }
			}
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComplex(It.Is<Type>(t => t == typeof(MonetaryAmount)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass)});
		}

		[Test]
		public void MappingThroughMock()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMapping(mapping);
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.Single().Should().Be.OfType<HbmProperty>();
			var propertyMapping = (HbmProperty)rc.Properties.Single();
			propertyMapping.Name.Should().Be.EqualTo("Amount");
			propertyMapping.Type.Should("The persistent type can't be inferred at this point (IUserType in NH)").Be.Null();
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.Complex<MonetaryAmount>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}
	}
}