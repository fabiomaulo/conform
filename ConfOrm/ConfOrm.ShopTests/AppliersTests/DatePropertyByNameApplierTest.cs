using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Appliers;
using Moq;
using NHibernate;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class DatePropertyByNameApplierTest
	{
		private class MyClass
		{
			public int Date { get; set; }
			public DateTime Moment { get; set; }
			public DateTime? CreatedAt { get; set; }
			public DateTime BirthDate { get; set; }
			public DateTime? DateOfExpiration { get; set; }
		}

		[Test]
		public void WhenMemberIsNotDateDateTimeThenNoMatch()
		{
			var applier = new DatePropertyByNameApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.Moment)).Should().Be.False();
		}

		[Test]
		public void WhenMemberIsNotDateNullableDateTimeThenNoMatch()
		{
			var applier = new DatePropertyByNameApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.CreatedAt)).Should().Be.False();
		}

		[Test]
		public void WhenMemberIsDateDateTimeThenMatch()
		{
			var applier = new DatePropertyByNameApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.BirthDate)).Should().Be.True();
		}

		[Test]
		public void WhenMemberIsDateNullableDateTimeThenMatch()
		{
			var applier = new DatePropertyByNameApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.DateOfExpiration)).Should().Be.True();
		}

		[Test]
		public void WhenMemberIsNotDateTimeThenNoMatch()
		{
			var applier = new DatePropertyByNameApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.Date)).Should().Be.False();
		}

		[Test]
		public void AlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IPropertyMapper>();
			var applier = new DatePropertyByNameApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.CreatedAt), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<IType>(t => t == NHibernateUtil.Date)));
		}

	}
}