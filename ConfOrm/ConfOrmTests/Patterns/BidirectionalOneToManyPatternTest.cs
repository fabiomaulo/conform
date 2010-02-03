using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyPatternTest
	{
		private class Parent
		{
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public Parent Parent { get; set; }
		}

		[Test]
		public void MatchOneToMany()
		{
			var pattern = new BidirectionalOneToManyPattern();
			pattern.Match(new Relation(typeof (Parent), typeof (Child))).Should().Be.True();
		}

		[Test]
		public void NoMatchManyToOne()
		{
			var pattern = new BidirectionalOneToManyPattern();
			pattern.Match(new Relation(typeof(Child), typeof(Parent))).Should().Be.False();
		}
	}
}