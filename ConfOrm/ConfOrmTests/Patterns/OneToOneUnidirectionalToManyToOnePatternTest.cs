using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class OneToOneUnidirectionalToManyToOnePatternTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyOneClass MyOneClass { get; set; }
		}

		private class MyOneClass
		{
			public int Id { get; set; }
		}
		private class MyOneBidirectional
		{
			public int Id { get; set; }
			public MyClass MyClass { get; set; }
		}

		[Test]
		public void WhenDeclaredExplicitOneToOneAndIsUnirectionalThenMatch()
		{
			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof (MyClass), typeof (MyOneClass));
			edo.OneToOneRelations.Add(relation);
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(edo);
			pattern.Match(relation).Should().Be.True();
		}

		[Test]
		public void WhenDeclaredExplicitOneToOneAndIsNotUnirectionalThenNoMatch()
		{
			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneBidirectional));
			edo.OneToOneRelations.Add(relation);
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(edo);
			pattern.Match(relation).Should().Be.False();
		}

		[Test]
		public void WhenNotDeclaredExplicitOneToOneThenNoMatch()
		{
			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneClass));
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(edo);
			pattern.Match(relation).Should().Be.False();
		}
	}
}