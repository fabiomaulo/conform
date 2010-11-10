using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.PatternTests
{
	public class InterfaceOnParentTest
	{
		private interface IParent
		{
			IEnumerable<Child> Children { get; set; }
		}

		private class Parent : IParent
		{
			public int Id { get; set; }
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public IParent Parent { get; set; }
		}

		[Test]
		public void WhenInterfaceOnParentThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.True();
		}
	}
}