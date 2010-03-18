using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyPatternGenericCircularTest
	{
		const BindingFlags RootClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		private MemberInfo parentProperty = typeof(ConcreteNode).GetProperty("Parent", RootClassPropertiesBindingFlags);
		private MemberInfo subnodesProperty = typeof(ConcreteNode).GetProperty("Subnodes", RootClassPropertiesBindingFlags);

		private class ConcreteNode : Node<ConcreteNode>
		{
		}

		private class Node<T> where T : Node<T>
		{
			public int Id { get; set; }
			public T Parent { get; set; }
			public ICollection<T> Subnodes { get; private set; }
		}

		[Test]
		public void WhenCircularThenOneToManyMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalOneToManyCascadePattern(orm.Object);
			pattern.Match(new RelationOn(typeof(ConcreteNode), subnodesProperty, typeof(ConcreteNode))).Should().Be.True();
		}

		[Test]
		public void WhenCircularThenManyToOneNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalOneToManyCascadePattern(orm.Object);
			pattern.Match(new RelationOn(typeof(ConcreteNode), parentProperty, typeof(ConcreteNode))).Should().Be.False();
		}
	}
}