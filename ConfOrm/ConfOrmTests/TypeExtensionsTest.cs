using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class TypeExtensionsTest
	{
		private const BindingFlags BindingFlagsIncludePrivate = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

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
			private double SomethingPrivate { get; set; }
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

		private interface IEntity<T>
		{
			T Id { get; set; }
		}
		private abstract class AbstractEntity<T> : IEntity<T>
		{
			public abstract T Id { get; set; }
			public abstract bool BaseBool { get; set; }
		}

		private class BaseEntity : AbstractEntity<int>
		{
			public override int Id { get; set; }

			public override bool BaseBool { get; set; }
		}
		private class MyEntity: BaseEntity
		{
		}

		[Test]
		public void DecodeMemberAccessExpressionOfWithGenericShouldReturnMemberOfRequiredClass()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.Id).Satisfy(
				mi => mi.ReflectedType == typeof(MyEntity) && mi.DeclaringType == typeof(BaseEntity));
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyEntity) && mi.DeclaringType == typeof(BaseEntity));
		}

		[Test]
		public void WhenBaseIsAbstractGenericGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("Id", typeof(int));
			var declaringMi = mi.GetMemberFromDeclaringType();
			declaringMi.DeclaringType.Should().Be<BaseEntity>();
			declaringMi.ReflectedType.Should().Be<BaseEntity>();
		}

		[Test]
		public void WhenBaseIsAbstractGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("BaseBool", typeof(bool));
			var declaringMi = mi.GetMemberFromDeclaringType();
			declaringMi.DeclaringType.Should().Be<BaseEntity>();
			declaringMi.ReflectedType.Should().Be<BaseEntity>();
		}

		[Test]
		public void GetFirstPropertyOfTypeWithNulls()
		{
			Type myType = null;
			myType.GetFirstPropertyOfType(typeof (int), BindingFlagsIncludePrivate).Should().Be.Null();
			myType = typeof (Array);
			myType.GetFirstPropertyOfType(null, BindingFlagsIncludePrivate).Should().Be.Null();
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyExistThenFindProperty()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string)).Should().Be(
				typeof (MyBaseClass).GetProperty("BaseProperty"));
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (bool)).Should().Be(typeof (MyBaseClass).GetProperty("BaseBool"));
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double), BindingFlagsIncludePrivate).Should().Be(
				typeof (MyBaseClass).GetProperty("SomethingPrivate", BindingFlagsIncludePrivate));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyNotExistThenNull()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (float)).Should().Be.Null();
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double)).Should().Be.Null();
		}
	}
}