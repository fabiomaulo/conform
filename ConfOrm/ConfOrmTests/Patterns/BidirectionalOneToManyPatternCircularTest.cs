using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.Patterns;
using Moq;
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
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyCascadeApplier(orm.Object);
			pattern.Match(subnodesProperty).Should().Be.True();
		}

		[Test]
		public void WhenCircularThenManyToOneNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyCascadeApplier(orm.Object);
			pattern.Match(parentProperty).Should().Be.False();
		}

		[Test]
		public void WhenNoCircularThenOneToManyMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyCascadeApplier(orm.Object);
			pattern.Match(ConfOrm.ForClass<Parent>.Property(p => p.Children)).Should().Be.True();
		}

		[Test]
		public void WhenNoCircularThenManyToOneNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyCascadeApplier(orm.Object);
			pattern.Match(ConfOrm.ForClass<Child>.Property(c => c.Parent)).Should().Be.False();
		}

		[Test]
		public void ApplyAlwaysReturnCascade()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyCascadeApplier(orm.Object);
			var collectionMapping = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(null, collectionMapping.Object);
			collectionMapping.Verify(cm=> cm.Cascade(It.Is<Cascade>(cascade=> cascade.Has(Cascade.All) && cascade.Has(Cascade.DeleteOrphans))));
		}
	}
}