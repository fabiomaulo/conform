using System;
using System.Collections.Generic;
using ConfOrm;
using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class HierarchyGenericClassExclusionTest
	{
		public abstract class Movement
		{
			public int Id { get; set; }
			public DateTime Date { get; set; }
		}
		public abstract class Movement<TDetail> : Movement
		{
			private ISet<TDetail> details;

			protected Movement()
			{
				details = new HashedSet<TDetail>();
			}

			public IEnumerable<TDetail> Details
			{
				get { return details; }
			}
		}
		public class Income : Movement<string>
		{
			public virtual string Origin { get; set; }
		}
		public class Outcome : Movement<int>
		{
			public string Branch { get; set; }
		}

		[Test]
		public void WhenExplicitExcludedUsingGenericTypeDefinitionThenNotIncludeInHierarchy()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Movement>();
			orm.Exclude(typeof(Movement<>));

			orm.IsRootEntity(typeof(Movement)).Should().Be.True();
			orm.IsRootEntity(typeof(Movement<>)).Should().Be.False();

			orm.IsEntity(typeof(Movement)).Should().Be.True();
			orm.IsEntity(typeof(Income)).Should().Be.True();
			orm.IsEntity(typeof(Outcome)).Should().Be.True();
			orm.IsEntity(typeof(Movement<string>)).Should().Be.False();
			orm.IsEntity(typeof(Movement<int>)).Should().Be.False();
		}
	}
}