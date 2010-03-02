using ConfOrm.Patterns;
using NUnit.Framework;
using TypeExtensions = ConfOrm.TypeExtensions;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PoidIntPatternTest
	{
		private class MyClass
		{
			public int IntProp { get; set; }
			public long LongProp { get; set; }
			public string StringProp { get; set; }
			public object ObjectProp { get; set; }
		}

		[Test]
		public void MatchWithIntMember()
		{
			var pattern = new PoidIntPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.IntProp)).Should().Be(true);
		}

		[Test]
		public void MatchWithLongMember()
		{
			var pattern = new PoidIntPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.LongProp)).Should().Be(true);
		}

		[Test]
		public void NoMatchWithOthersTypes()
		{
			var pattern = new PoidIntPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.StringProp)).Should().Be(false);
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.ObjectProp)).Should().Be(false);
		}

		[Test]
		public void NoMatchWithNullMember()
		{
			var pattern = new PoidIntPattern();
			pattern.Match(null).Should().Be(false);
		}
	}
}