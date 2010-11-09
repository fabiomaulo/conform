using System.Collections;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.PatternTests
{
	public class NoPolymorphicParentChildTest
	{
		private class Parent
		{
			public int Id { get; set; }
			public IEnumerable<Child> Children { get; set; }
			public IEnumerable Whatever { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public Parent Parent { get; set; }
		}

		[Test]
		public void WhenNullThenNoThrow()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Executing(p=> p.Match(null)).NotThrows();
		}

		[Test]
		public void WhenNoGenericThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Parent>.Property(x => x.Whatever)).Should().Be.False();
		}

		[Test]
		public void WhenNoPolymorphicThenNoMatch()
		{
			// using the concrete implementation of IDomainInspector this test is more like an integration-test but it is exactly what I need.
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			pattern.Match(ForClass<Parent>.Property(x => x.Children)).Should().Be.False();
		}
	}
}