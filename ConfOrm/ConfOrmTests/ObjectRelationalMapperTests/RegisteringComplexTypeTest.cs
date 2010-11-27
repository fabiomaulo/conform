using System;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class RegisteringComplexTypeTest
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

		[Test]
		public void WhenRegisterNullThenThrows()
		{
			var orm = new ObjectRelationalMapper();
			orm.Executing(o => o.Complex(null)).Throws<ArgumentNullException>();
		}

		[Test]
		public void WhenRegisterTypeThenAddToExplicitHolder()
		{
			var orm = new ObjectRelationalMapper();
			orm.Complex(typeof(MonetaryAmount));
			orm.IsComplex(ForClass<MyClass>.Property(x => x.Amount)).Should().Be.True();
		}
	}
}