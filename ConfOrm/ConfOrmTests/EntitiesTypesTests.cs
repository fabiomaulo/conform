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
	}
}