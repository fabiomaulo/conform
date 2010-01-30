using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PropertyToFieldCamelCasePatternTest
	{
		private class A
		{
			private string aProperty;
			private string _BProperty;

			public string AProperty
			{
				get { return aProperty; }
			}
			public string BProperty
			{
				get { return _BProperty; }
			}
		}

		[Test]
		public void MatchPropertyWithCamelCaseBackField()
		{
			var prop = typeof (A).GetProperty("AProperty");
			var p = new PropertyToFieldCamelCasePattern();
			p.Match(prop).Should().Be.True();
		}

		[Test]
		public void NoMatchPropertyWithNoCamelCaseBackField()
		{
			var prop = typeof(A).GetProperty("BProperty");
			var p = new PropertyToFieldCamelCasePattern();
			p.Match(prop).Should().Be.False();
		}
	}
}