using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PropertyToFieldMUnderscorePascalCasePatternTest
	{
		private class A
		{
			private string m_AProperty;
			private string bProperty;

			public string AProperty
			{
				get { return m_AProperty; }
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
			var p = new PropertyToFieldMUnderscorePascalCasePattern();
			p.Match(prop).Should().Be.True();
		}

		[Test]
		public void NoMatchPropertyWithNoCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("BProperty");
			var p = new PropertyToFieldMUnderscorePascalCasePattern();
			p.Match(prop).Should().Be.False();
		}
	}
}