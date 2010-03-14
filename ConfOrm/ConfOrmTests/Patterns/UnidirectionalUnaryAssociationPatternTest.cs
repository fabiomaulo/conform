using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class UnidirectionalUnaryAssociationPatternTest
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
			var pattern = new UnidirectionalUnaryAssociationPattern();
			pattern.Match(null).Should().Be.False();
		}

		[Test]
		public void WhenNoBidirectionalThenShouldMatch()
		{
			var pattern = new UnidirectionalUnaryAssociationPattern();
			pattern.Match(typeof(MyClass).GetProperty("A")).Should().Be.True();
		}

		[Test]
		public void WhenBidirectionalThenShouldntMatch()
		{
			var pattern = new UnidirectionalUnaryAssociationPattern();
			pattern.Match(typeof(MyClass).GetProperty("MyClassOther")).Should().Be.False();
			pattern.Match(typeof(MyClassOther).GetProperty("MyClass")).Should().Be.False();
		}
	}
}