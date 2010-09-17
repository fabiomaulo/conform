using ConfOrm.Mappers;
using ConfOrm.Shop.Appliers;
using Moq;
using NHibernate;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class UseCurrencyForDecimalApplierTest
	{
		private class MyClass
		{
			public int Value { get; set; }
			public decimal Price { get; set; }
		}

		[Test]
		public void WhenNullThenNotThrows()
		{
			var applier = new UseCurrencyForDecimalApplier();
			applier.Executing(a => a.Match(null)).NotThrows();
		}

		[Test]
		public void WhenNoDecimalThenNoMatch()
		{
			var applier = new UseCurrencyForDecimalApplier();
			applier.Match(ForClass<MyClass>.Property(mc => mc.Value)).Should().Be.False();
		}

		[Test]
		public void WhenDecimalThenMatch()
		{
			var applier = new UseCurrencyForDecimalApplier();
			applier.Match(ForClass<MyClass>.Property(mc => mc.Price)).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyCurrencyType()
		{
			var propertyMapper = new Mock<IPropertyMapper>();
			var applier = new UseCurrencyForDecimalApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.Price), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<IType>(t => t == NHibernateUtil.Currency)));
		}
	}
}