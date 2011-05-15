using System;
using NHibernate.Mapping.ByCode;
using ConfOrm.Shop.Appliers;
using Moq;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class EnumPropertyAsStringApplierTest
	{
		private enum MyEnum
		{
			One
		}

		[Flags]
		private enum MyFlagEnum
		{
			One = 1
		}

		private class MyClass
		{
			public int AnyProp { get; set; }
			public MyEnum MyEnum { get; set; }
			public MyEnum? MyEnumNullable { get; set; }
			public MyFlagEnum MyFlagEnum { get; set; }
			public MyFlagEnum? MyFlagEnumNullable { get; set; }
		}

		[Test]
		public void WhenPropIsEnumThenMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c=> c.MyEnum)).Should().Be.True();
		}

		[Test]
		public void WhenPropIsFlagEnumThenNoMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyFlagEnum)).Should().Be.False();
		}

		[Test]
		public void WhenPropIsNullableEnumThenMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyEnumNullable)).Should().Be.True();
		}

		[Test]
		public void WhenPropIsFlagNullableEnumThenNoMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyFlagEnumNullable)).Should().Be.False();
		}

		[Test]
		public void WhenPropIsNoEnumThenNoMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.AnyProp)).Should().Be.False();
		}

		[Test]
		public void AlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IPropertyMapper>();
			var applier = new EnumPropertyAsStringApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.MyEnum), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<Type>(t => t == typeof(EnumStringType<MyEnum>)), It.Is<object>(p=> p == null)));
		}

		[Test]
		public void WhenNullableEnumAlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IPropertyMapper>();
			var applier = new EnumPropertyAsStringApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.MyEnumNullable), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<Type>(t => t == typeof(EnumStringType<MyEnum?>)), It.Is<object>(p => p == null)));
		}
	}
}