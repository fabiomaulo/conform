using System.Collections.Generic;
using ConfOrm.Shop.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.PatternsTests
{
	public class UseSetWhenGenericCollectionPatternTest
	{
		private class MyClass
		{
			public string StringProp { get; set; }
			public byte[] Bytes { get; set; }
			public IEnumerable<string >	Enumerable { get; set; }
			public IList<string>	List { get; set; }
			public string[]	Array { get; set; }
			public ICollection<string>	Collection { get; set; }
		}

		[Test]
		public void WhenStringThenNoMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.StringProp);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenByteArrayThenNoMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.Bytes);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenEnumerableThenNoMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.Enumerable);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenListThenNoMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.List);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenArrayThenNoMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.Array);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenGenericCollectionThenMatch()
		{
			var member = ForClass<MyClass>.Property(x => x.Collection);
			var pattern = new UseSetWhenGenericCollectionPattern();
			pattern.Match(member).Should().Be.True();
		}
	}
}