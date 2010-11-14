using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.AnyToMany
{
	public class Case6RootEntitiesPatternTests
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelatedRoot RelatedRoot { get; set; }
		}

		private interface IRelatedRoot
		{

		}

		private class MyRelatedRoot1 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}

		private class MyRelatedRoot2 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}

		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new PolymorphismBidirectionalAnyToManyPattern(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenSubjectIsNullThenNotThrow()
		{
			var orm = new ObjectRelationalMapper();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Executing(p=> p.Match(null)).NotThrows();
		}

		[Test]
		public void WhenManySideIsNotAnEntityThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyRelatedRoot1>();
			orm.TablePerClass<MyRelatedRoot2>();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedRoot1>.Property(x => x.Items)).Should().Be.False();
		}

		[Test]
		public void WhenTheDomainDoesNotContainMoreImplsThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelatedRoot1>();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedRoot1>.Property(x => x.Items)).Should().Be.False();
		}

		[Test]
		public void WhenTheDomainContainsMoreImplsThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelatedRoot1>();
			orm.TablePerClass<MyRelatedRoot2>();

			var pattern = new PolymorphismBidirectionalAnyToManyPattern(orm);
			pattern.Match(ForClass<MyRelatedRoot1>.Property(x => x.Items)).Should().Be.True();
		}
	}
}