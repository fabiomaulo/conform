using System;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class EntitiesTypesTests
	{
		public struct MyStruct
		{
			public int Something;
		}

		public enum ShortTypes: short
		{
			Unknown = -1,
			Post = 0,
			Contribute = 5,
			Page = 7
		}
		public enum LongEnum : long
		{
			Something,
		}
		public enum ULongEnum : ulong
		{
			Something,
		}
		public enum IntEnum 
		{
			Something,
		}
		public enum UIntEnum : uint
		{
			Something,
		}
		public enum UShortEnum : ushort
		{
			Something,
		}
		public enum ByteEnum : byte
		{
			Something,
		}
		public enum SbyteEnum : sbyte
		{
			Something,
		}

		[Test]
		public void WhenTypeOrNameIsNullThenThrows()
		{
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(null, "blah")).Should().Throw<ArgumentNullException>();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof (ShortTypes), null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenTypeIsNotEnumThenThrows()
		{
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(MyStruct), "Something")).Should().Throw<ArgumentException>();
		}

		[Test]
		public void WhenNameFoundThenReturnValue()
		{
			EnumUtil.ParseGettingUnderlyingValue(typeof(ShortTypes), "Contribute").Should().Be((short)5);
		}

		[Test]
		public void WhenNameNotFoundThenThrows()
		{
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(ShortTypes), "Pizza")).Should().Throw<ArgumentException>();
		}

		[Test]
		public void WorkWithAllTypesSupportedByEnum()
		{
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(LongEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(ULongEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(IntEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(UIntEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(UShortEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(ByteEnum), "Something")).Should().NotThrow();
			Executing.This(() => EnumUtil.ParseGettingUnderlyingValue(typeof(SbyteEnum), "Something")).Should().NotThrow();
		}
	}
}