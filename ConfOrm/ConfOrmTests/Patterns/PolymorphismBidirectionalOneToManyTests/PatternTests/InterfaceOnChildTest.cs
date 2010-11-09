using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.PatternTests
{
	public class InterfaceOnChildTest
	{
		private interface IChild
		{
			Parent Parent { get; set; }
		}

		private class Parent
		{
			public int Id { get; set; }
			public IEnumerable<IChild> Children { get; set; }
		}

		private class Child : IChild
		{
			public int Id { get; set; }
			public Parent Parent { get; set; }
		}

		[Test]
		public void WhenInterfaceOnChildThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Parent>.Property(x=> x.Children)).Should().Be.True();
		}
	}
}