using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.UnidirectionalOneToManyMultipleCollections
{
	public class CircularReferenceTest
	{
		private class Node
		{
			public Node Parent { get; set; }
			public IEnumerable<Node> Nodes { get; set; }
		}

		private interface INode
		{
			INode Parent { get; set; }
			IEnumerable<INode> Nodes { get; set; }
		}

		private class PolyNode: INode
		{
			public INode Parent { get; set; }
			public IEnumerable<INode> Nodes { get; set; }
		}

		private class PolyNodeDouble : INode
		{
			public INode Parent { get; set; }
			public IEnumerable<INode> Nodes { get; set; }
			public IEnumerable<INode> OthersNodes { get; set; }
		}

		[Test]
		public void WhenSingleCirsularRelationOnEntityThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(dm => dm.IsEntity(It.Is<Type>(t => t == typeof(Node)))).Returns(true);
			orm.Setup(dm => dm.IsManyToOne(typeof(Node), typeof(Node))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(Node), typeof(Node))).Returns(true);

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<Node>.Property(x => x.Nodes));
			applier.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenSingleCirsularRelationOnComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(dm => dm.IsOneToMany(typeof(Node), typeof(Node))).Returns(true);

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<Node>.Property(x => x.Nodes));
			applier.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenSingleCirsularRelationOnPolymorphicComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(dm => dm.IsEntity(It.Is<Type>(t => t == typeof(PolyNode)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(typeof(INode))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(PolyNode), typeof(INode))).Returns(true);

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<PolyNode>.Property(x => x.Nodes));
			applier.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenDoubleCirsularRelationOnPolymorphicComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(dm => dm.IsEntity(It.Is<Type>(t => t == typeof(PolyNodeDouble)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(typeof(INode))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(PolyNodeDouble), typeof(INode))).Returns(true);

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<PolyNodeDouble>.Property(x => x.Nodes));
			applier.Match(property).Should().Be.True();
		}
	}
}