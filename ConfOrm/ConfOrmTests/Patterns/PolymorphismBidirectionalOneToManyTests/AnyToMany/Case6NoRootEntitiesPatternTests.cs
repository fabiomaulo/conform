using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.AnyToMany
{
	public class Case6NoRootEntitiesPatternTests
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelated Related { get; set; }
		}

		private interface IRelated
		{

		}

		private class MyRoot1
		{
			public int Id { get; set; }
		}

		private class MyRelatedNoRoot1 : MyRoot1, IRelated
		{
			public IEnumerable<MyEntity> Items { get; set; }
		}

		private class MyRoot2
		{
			public int Id { get; set; }
		}

		private class MyRelatedNoRoot2 : MyRoot2, IRelated
		{
			public IEnumerable<MyEntity> Items { get; set; }
		}

		[Test]
		public void WhenManySideIsNotAnEntityThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyRoot1>();
			orm.TablePerClass<MyRoot2>();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedNoRoot1>.Property(x => x.Items)).Should().Be.False();
		}

		[Test]
		public void WhenTheDomainDoesNotContainMoreImplsThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRoot1>();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedNoRoot1>.Property(x => x.Items)).Should().Be.False();
		}

		[Test]
		public void WhenTheDomainContainsMoreImplsThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRoot1>();
			orm.TablePerClass<MyRoot2>();

			orm.AddToDomain(typeof(MyRelatedNoRoot1));
			orm.AddToDomain(typeof(MyRelatedNoRoot2));

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedNoRoot1>.Property(x => x.Items)).Should().Be.True();
			pattern.Match(ForClass<MyRelatedNoRoot2>.Property(x => x.Items)).Should().Be.True();
		}
	}
}