using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.PatternTests
{
	public class CircularReferenceTest
	{
		private interface INode
		{
			INode ParentNode { get; set; }
			IEnumerable<INode> SubNodes { get; set; }
		}

		private class Node : INode
		{
			public int Id { get; set; }
			public INode ParentNode { get; set; }
			public IEnumerable<INode> SubNodes { get; set; }
		}

		[Test]
		public void WhenInterfaceOnParentThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Node>.Property(x => x.SubNodes)).Should().Be.True();
		}

		[Test]
		public void WhenInterfaceOnParentAndPropertyExclusionOnInterfaceThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.ExcludeProperty<INode>(x => x.ParentNode);

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Node>.Property(x => x.SubNodes)).Should().Be.False();
		}

		[Test]
		public void WhenInterfaceOnParentAndPropertyExclusionOnImplThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.ExcludeProperty<Node>(x => x.ParentNode);

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Node>.Property(x => x.SubNodes)).Should().Be.False();
		}
	}
}