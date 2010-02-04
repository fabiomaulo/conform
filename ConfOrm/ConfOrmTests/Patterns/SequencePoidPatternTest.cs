using System;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class SequencePoidPatternTest
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
			var pattern = new SequencePoidPattern();
			pattern.Match(typeof(TestEntity).GetProperty("Int")).Should().Be.True();
			pattern.Match(typeof(TestEntity).GetProperty("Long")).Should().Be.True();
		}

		[Test]
		public void NoMatchWithOthers()
		{
			var pattern = new SequencePoidPattern();
			pattern.Match(typeof(TestEntity).GetProperty("Guid")).Should().Be.False();
			pattern.Match(typeof(TestEntity).GetProperty("Short")).Should().Be.False();
		}

		[Test]
		public void ApplyHasSequenceGeneratorNoParams()
		{
			var pattern = new SequencePoidPattern();
			pattern.Apply(typeof(TestEntity).GetProperty("Int")).Satisfy(
				poidi => poidi.Strategy == PoIdStrategy.Sequence && poidi.Params == null);
		}

		[Test]
		public void ApplyHasSequenceGeneratorParams()
		{
			var pattern = new SequencePoidPattern(new { sequence = "mySequence" });
			pattern.Apply(typeof(TestEntity).GetProperty("Int")).Satisfy(
				poidi => poidi.Strategy == PoIdStrategy.Sequence && poidi.Params != null);
		}
	}
}