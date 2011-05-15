using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.Shop.Appliers;
using Moq;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class EnumElementAsStringApplierTest
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
			public IEnumerable<int> AnyProp { get; set; }
			public IEnumerable<MyEnum> MyEnum { get; set; }
			public IEnumerable<MyEnum?> MyEnumNullable { get; set; }
			public IDictionary<string, MyEnum> MapMyEnumValue { get; set; }
			public IDictionary<MyEnum, string> MapMyEnumKey { get; set; }
			public IEnumerable<MyFlagEnum> MyFlagEnum { get; set; }
			public IEnumerable<MyFlagEnum?> MyFlagEnumNullable { get; set; }
		}

		[Test]
		public void WhenCollectionOfEnumThenMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyEnum)).Should().Be.True();
		}

		[Test]
		public void WhenCollectionOfFlagEnumThenNoMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyFlagEnum)).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfNullableEnumThenMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyEnumNullable)).Should().Be.True();
		}

		[Test]
		public void WhenCollectionOfFlagNullableEnumThenNoMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MyFlagEnumNullable)).Should().Be.False();
		}

		[Test]
		public void WhenDictionaryOfEnumOnValueThenMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MapMyEnumValue)).Should().Be.True();
		}

		[Test]
		public void WhenDictionaryOfEnumOnKeyThenNoMatch()
		{
			var applier = new EnumElementAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.MapMyEnumKey)).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfNoEnumThenNoMatch()
		{
			var applier = new EnumPropertyAsStringApplier();
			applier.Match(ForClass<MyClass>.Property(c => c.AnyProp)).Should().Be.False();
		}

		[Test]
		public void AlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IElementMapper>();
			var applier = new EnumElementAsStringApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.MyEnum), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<Type>(t => t == typeof(EnumStringType<MyEnum>)), It.Is<object>(p => p == null)));
		}

		[Test]
		public void WhenNullableEnumAlwaysApplyColumnType()
		{
			var propertyMapper = new Mock<IElementMapper>();
			var applier = new EnumElementAsStringApplier();
			applier.Apply(ForClass<MyClass>.Property(x => x.MyEnumNullable), propertyMapper.Object);

			propertyMapper.Verify(x => x.Type(It.Is<Type>(t => t == typeof(EnumStringType<MyEnum?>)), It.Is<object>(p => p == null)));
		}
	}
}