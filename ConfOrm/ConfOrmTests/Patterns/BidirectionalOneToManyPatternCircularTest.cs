using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyPatternCircularTest
	{
		private MemberInfo parentProperty = typeof(Node).GetProperty("Parent");
		private MemberInfo subnodesProperty = typeof(Node).GetProperty("Subnodes");

		private class Node
		{
			public Node Parent { get; set; }
			public IEnumerable<Node> Subnodes { get; set; }
		}

		private class Parent
		{
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public Parent Parent { get; set; }
		}

		[Test]
		public void WhenCircularThenOneToManyMatch()
		{
			var pattern = new BidirectionalOneToManyCascadePattern();
			pattern.Match(new RelationOn(typeof(Node), subnodesProperty, typeof(Node))).Should().Be.True();
		}

		[Test]
		public void WhenCircularThenManyToOneNoMatch()
		{
			var pattern = new BidirectionalOneToManyCascadePattern();
			pattern.Match(new RelationOn(typeof(Node), parentProperty, typeof(Node))).Should().Be.False();
		}

		[Test]
		public void WhenNoCircularThenOneToManyMatch()
		{
			var pattern = new BidirectionalOneToManyCascadePattern();
			pattern.Match(new Relation(typeof(Parent), typeof(Child))).Should().Be.True();
		}

		[Test]
		public void WhenNoCircularThenManyToOneNoMatch()
		{
			var pattern = new BidirectionalOneToManyCascadePattern();
			pattern.Match(new Relation(typeof(Child), typeof(Parent))).Should().Be.False();
		}

		[Test]
		public void ApplyAlwaysReturnCascade()
		{
			var pattern = new BidirectionalOneToManyCascadePattern();
			pattern.Get(null).Satisfy(c => c.Has(Cascade.All) && c.Has(Cascade.DeleteOrphans));
		}
	}
}