using System.Collections.Generic;
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
	}
}