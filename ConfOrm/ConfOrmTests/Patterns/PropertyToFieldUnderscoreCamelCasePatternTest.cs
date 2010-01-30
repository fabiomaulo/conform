using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PropertyToFieldUnderscoreCamelCasePatternTest
	{
		private class A
		{
			private string _aProperty;
			private string _BProperty;

			public string AProperty
			{
				get { return _aProperty; }
			}
			public string BProperty
			{
				get { return _BProperty; }
			}
		}

		[Test]
		public void MatchPropertyWithCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("AProperty");
			var p = new PropertyToFieldUnderscoreCamelCasePattern();
			p.Match(prop).Should().Be.True();
		}

		[Test]
		public void NoMatchPropertyWithNoCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("BProperty");
			var p = new PropertyToFieldUnderscoreCamelCasePattern();
			p.Match(prop).Should().Be.False();
		}
	}
}