using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class TypeExtensionsTest
	{
		[Test]
		public void CanDetermineDictionaryKeyType()
		{
			typeof (IDictionary<string, int>).DetermineDictionaryKeyType().Should().Be.EqualTo<string>();
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryKeyType()
		{
			typeof(IEnumerable<string>).DetermineDictionaryKeyType().Should().Be.Null();
		}

		[Test]
		public void CanDetermineDictionaryValueType()
		{
			typeof(IDictionary<string, int>).DetermineDictionaryValueType().Should().Be.EqualTo<int>();
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryValueType()
		{
			typeof(IEnumerable<string>).DetermineDictionaryValueType().Should().Be.Null();
		}

		private class MyBaseClass
		{
			public string BaseProperty { get; set; }
			public bool BaseBool { get; set; }
		}
		private class MyClass : MyBaseClass
		{
			
		}

		[Test]
		public void DecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof (MyClass) && mi.DeclaringType == typeof (MyBaseClass));
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void GetBaseTypesIncludesInterfaces()
		{
			typeof (Collection<>).GetBaseTypes().Should().Contain(typeof (IEnumerable));
		}
	}
}