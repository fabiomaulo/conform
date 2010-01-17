using System;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PoIdGuidStrategyPatternTest
	{
		private class TestEntity
		{
			public int Id { get; set; }
			public Guid PoId { get; set; }
		}

		[Test]
		public void PatternMatch()
		{
			var pattern = new PoIdGuidStrategyPattern();
			PropertyInfo pi = typeof(TestEntity).GetProperty("Id");
			pi.Satisfy(p => !pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("PoId");
			pi.Satisfy(p => pattern.Match(p));
		}
	}
}