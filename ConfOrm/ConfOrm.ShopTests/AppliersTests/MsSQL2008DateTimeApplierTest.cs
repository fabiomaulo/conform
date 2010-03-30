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
	public class MsSQL2008DateTimeApplierTest
	{
		private class MyClass
		{
			public int Something { get; set; }
			public DateTime Moment { get; set; }
			public DateTime? CreatedAt { get; set; }
			public DateTime BirthDate { get; set; }
			public DateTime? DateOfExpiration { get; set; }
		}

		[Test]
		public void WhenMemberIsNotDateDateTimeThenMatch()
		{
			var applier = new MsSQL2008DateTimeApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.Moment)).Should().Be.True();
		}

		[Test]
		public void WhenMemberIsNotDateNullableDateTimeThenMatch()
		{
			var applier = new MsSQL2008DateTimeApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.CreatedAt)).Should().Be.True();
		}

		[Test]
		public void WhenMemberIsDateDateTimeThenNoMatch()
		{
			var applier = new MsSQL2008DateTimeApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.BirthDate)).Should().Be.False();
		}

		[Test]
		public void WhenMemberIsDateNullableDateTimeThenNoMatch()
		{
			var applier = new MsSQL2008DateTimeApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.DateOfExpiration)).Should().Be.False();
		}

		[Test]
		public void WhenMemberIsNotDateTimeThenNoMatch()
		{
			var applier = new MsSQL2008DateTimeApplier();
			applier.Match(ForClass<MyClass>.Property(x => x.Something)).Should().Be.False();
		}

		[Test]
		public void AlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IPropertyMapper>();
			var applier = new MsSQL2008DateTimeApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.CreatedAt), propertyMapper.Object);

			propertyMapper.Verify(x=> x.Type(It.Is<IType>(t=> t == NHibernateUtil.DateTime2)));
		}
	}
}