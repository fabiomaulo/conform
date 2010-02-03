using System;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class HighLowPoidPatternTest
	{
		private class TestEntity
		{
			public int Int { get; set; }
			public long Long { get; set; }
			public Guid Guid { get; set; }
			public short Short { get; set; }
		}

		[Test]
		public void MatchWithAnyIntOrLong()
		{
			var pattern = new HighLowPoidPattern();
			pattern.Match(typeof (TestEntity).GetProperty("Int")).Should().Be.True();
			pattern.Match(typeof(TestEntity).GetProperty("Long")).Should().Be.True();
		}

		[Test]
		public void NoMatchWithOthers()
		{
			var pattern = new HighLowPoidPattern();
			pattern.Match(typeof(TestEntity).GetProperty("Guid")).Should().Be.False();
			pattern.Match(typeof(TestEntity).GetProperty("Short")).Should().Be.False();
		}

		[Test]
		public void ApplyHasHighLowGeneratorNoParams()
		{
			var pattern = new HighLowPoidPattern();
			pattern.Apply(typeof (TestEntity).GetProperty("Int")).Satisfy(
				poidi => poidi.Strategy == PoIdStrategy.HighLow && poidi.Params == null);
		}

		[Test]
		public void ApplyHasHighLowGeneratorParams()
		{
			var pattern = new HighLowPoidPattern(new {max_lo = 99});
			pattern.Apply(typeof(TestEntity).GetProperty("Int")).Satisfy(
				poidi => poidi.Strategy == PoIdStrategy.HighLow && poidi.Params != null);
		}
	}
}