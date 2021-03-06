using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.TypeExtensionsTests
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
		public void DecodeMemberAccessExpressionShouldReturnMemberOfDeclaringClass()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
			ConfOrm.TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void GenericDecodeMemberAccessExpressionShouldReturnMemberOfDeclaringClass()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpression<MyClass, string>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
			ConfOrm.TypeExtensions.DecodeMemberAccessExpression<MyClass, bool>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
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
		public void GenericDecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, string>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyClass) && mi.DeclaringType == typeof(MyBaseClass));
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, bool>(mc => mc.BaseBool).Satisfy(
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
			// typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double)).Should().Be.Null(); <= by default check private prop.
		}

		private interface IMyEntity : IEntity<Guid>
		{

		}

		[Test]
		public void WhenDecodeMemberAccessExpressionOfOnInheritedEntityInterfaceThenDecodeMember()
		{
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity>(m => m.Id).Should().Not.Be.Null();
			ConfOrm.TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity, Guid>(m => m.Id).Should().Not.Be.Null();
		}

		[Test]
		public void TheSequenceOfGetHierarchyFromBaseShouldStartFromBaseClassUpToGivenClass()
		{
			// excluding System.Object
			typeof(MyEntity).GetHierarchyFromBase().Should().Have.SameSequenceAs(typeof(AbstractEntity<int>), typeof(BaseEntity), typeof(MyEntity));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenDelegateIsNullThenThrow()
		{
			var myType = typeof(Array);
			Executing.This(()=> myType.GetFirstPropertyOfType(typeof(int), BindingFlagsIncludePrivate, null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void GetFirstPropertyOfType_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => false).Should().Be.Null();
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => true).Should().Be(
				typeof (MyBaseClass).GetProperty("BaseProperty"));
		}

		[Test]
		public void HasPublicPropertyOf_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => false).Should().Be.False();
			typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => true).Should().Be.True();
		}
	}
}