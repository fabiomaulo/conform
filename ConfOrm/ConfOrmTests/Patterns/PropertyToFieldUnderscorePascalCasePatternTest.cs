using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PropertyToFieldUnderscorePascalCasePatternTest
	{
		private class A
		{
			private string _AProperty;
			private string bProperty;

			public string AProperty
			{
				get { return _AProperty; }
			}
			public string BProperty
			{
				get { return bProperty; }
			}
		}

		[Test]
		public void MatchPropertyWithCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("AProperty");
			var p = new PropertyToFieldUnderscorePascalCasePattern();
			p.Match(prop).Should().Be.True();
		}

		[Test]
		public void NoMatchPropertyWithNoCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("BProperty");
			var p = new PropertyToFieldUnderscorePascalCasePattern();
			p.Match(prop).Should().Be.False();
		}
	}
}