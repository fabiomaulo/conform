using System;
using System.Collections.Generic;
using System.Data;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.Patterns;
using Moq;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class CustomUserTypeInDictionaryKeyApplierTest
	{
		private class MyType
		{

		}

		private class MyTypeUserType : IUserType
		{
			public bool Equals(object x, object y)
			{
				throw new NotImplementedException();
			}

			public int GetHashCode(object x)
			{
				throw new NotImplementedException();
			}

			public object NullSafeGet(IDataReader rs, string[] names, object owner)
			{
				throw new NotImplementedException();
			}

			public void NullSafeSet(IDbCommand cmd, object value, int index)
			{
				throw new NotImplementedException();
			}

			public object DeepCopy(object value)
			{
				throw new NotImplementedException();
			}

			public object Replace(object original, object target, object owner)
			{
				throw new NotImplementedException();
			}

			public object Assemble(object cached, object owner)
			{
				throw new NotImplementedException();
			}

			public object Disassemble(object value)
			{
				throw new NotImplementedException();
			}

			public SqlType[] SqlTypes
			{
				get { throw new NotImplementedException(); }
			}

			public Type ReturnedType
			{
				get { throw new NotImplementedException(); }
			}

			public bool IsMutable
			{
				get { throw new NotImplementedException(); }
			}
		}

		private class MyClass
		{
			public MyType MyType { get; set; }
			public IEnumerable<MyType> Enumerable { get; set; }
			public IEnumerable<string> EnumerableString { get; set; }
			public IDictionary<string, MyType> Dictionary { get; set; }
			public IDictionary<MyType, string> DictionaryString { get; set; }
		}

		[Test]
		public void DoesNotAllowNullsInCtor()
		{
			Executing.This(() => new CustomUserTypeInDictionaryKeyApplier(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new CustomUserTypeInDictionaryKeyApplier(null, typeof(MyTypeUserType))).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void ShouldCheckIUserTypeOnCtor()
		{
			Executing.This(() => new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(string))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenNotCollectionShouldNotMatch()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			pattern.Match(ForClass<MyClass>.Property(x => x.MyType)).Should().Be.False();
		}

		[Test]
		public void WhenUsedInCollectionShouldNoMatch()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			pattern.Match(ForClass<MyClass>.Property(x => x.Enumerable)).Should().Be.False();
		}

		[Test]
		public void WhenNotUsedInCollectionShouldNotMatch()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			pattern.Match(ForClass<MyClass>.Property(x => x.EnumerableString)).Should().Be.False();
		}

		[Test]
		public void WhenUsedInDictionaryValueShouldNoMatch()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			pattern.Match(ForClass<MyClass>.Property(x => x.Dictionary)).Should().Be.False();
		}

		[Test]
		public void WhenUsedInDictionaryKeyShouldMatch()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			pattern.Match(ForClass<MyClass>.Property(x => x.DictionaryString)).Should().Be.True();
		}

		[Test]
		public void ApplyShouldAlwaysSetIUserType()
		{
			var pattern = new CustomUserTypeInDictionaryKeyApplier(typeof(MyType), typeof(MyTypeUserType));
			var mapper = new Mock<IMapKeyMapper>();
			pattern.Apply(null, mapper.Object);

			mapper.Verify(x => x.Type(It.Is<Type>(t => t == typeof(MyTypeUserType))));
		}
	}
}