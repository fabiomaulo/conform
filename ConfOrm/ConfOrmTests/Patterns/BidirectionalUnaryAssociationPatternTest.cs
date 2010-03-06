using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalUnaryAssociationPatternTest
	{
		private class MyClass
		{
			public A A { get; set; }
			public MyClassOther MyClassOther { get; set; }
		}

		private class MyClassOther
		{
			public MyClass MyClass { get; set; }
		}
		private class A
		{
		}

		[Test]
		public void WhenNullMemberThenShouldntMatch()
		{
			var pattern = new BidirectionalUnaryAssociationPattern();
			pattern.Match(null).Should().Be.False();
		}

		[Test]
		public void WhenNoBidirectionalThenShouldntMatch()
		{
			var pattern = new BidirectionalUnaryAssociationPattern();
			pattern.Match(typeof(MyClass).GetProperty("A")).Should().Be.False();
		}

		[Test]
		public void WhenBidirectionalThenShouldMatch()
		{
			var pattern = new BidirectionalUnaryAssociationPattern();
			pattern.Match(typeof(MyClass).GetProperty("MyClassOther")).Should().Be.True();
			pattern.Match(typeof(MyClassOther).GetProperty("MyClass")).Should().Be.True();
		}
	}
}