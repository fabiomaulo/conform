using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class BidirectionalOneToManyCascadeCircularTest
	{
		private MemberInfo parentProperty = typeof(Node).GetProperty("Parent");
		private MemberInfo subnodesProperty = typeof(Node).GetProperty("Subnodes");

		private class Node
		{
			public int Id { get; set; }
			public Node Parent { get; set; }
			public IEnumerable<Node> Subnodes { get; set; }
		}

		[Test]
		public void ApplyCascadeOnChildByDefault()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.ApplyCascade(typeof(Node), subnodesProperty, typeof(Node)).Satisfy(c => c.Has(Cascade.All) && c.Has(Cascade.DeleteOrphans));
		}

		[Test]
		public void DoesNotApplyCascadeOnParentByDefault()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.ApplyCascade(typeof(Node), parentProperty, typeof(Node)).Should().Be.EqualTo(Cascade.None);
		}

		[Test]
		public void WhenExplicitRequiredApplyCascadeOnParent()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.Cascade<Node, Node>(n => n.Parent, Cascade.Persist);
			orm.ApplyCascade(typeof(Node), parentProperty, typeof(Node)).Should().Be.EqualTo(Cascade.Persist);
		}

		[Test]
		public void WhenExplicitRequiredByClassApplyCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.Cascade<Node, Node>(Cascade.Persist);
			orm.ApplyCascade(typeof(Node), parentProperty, typeof(Node)).Should().Be.EqualTo(Cascade.Persist);
		}
	}
}