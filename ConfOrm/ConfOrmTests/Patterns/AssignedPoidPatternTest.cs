using System;
using System.Linq;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class AssignedPoidPatternTest
	{
		private class TestEntity
		{
			public int Int { get; set; }
			public long Long { get; set; }
			public Guid Guid { get; set; }
			public short Short { get; set; }
		}

		[Test]
		public void MatchWithAny()
		{
			var pattern = new AssignedPoidPattern();
			typeof (TestEntity).GetProperties().All(prop=> prop.Satisfy(pp=> pattern.Match(pp)));
		}

		[Test]
		public void ApplyAlwaysReturnAssigned()
		{
			var pattern = new AssignedPoidPattern();
			pattern.Apply(null).Strategy.Should().Be.EqualTo(PoIdStrategy.Assigned);
		}
	}
}