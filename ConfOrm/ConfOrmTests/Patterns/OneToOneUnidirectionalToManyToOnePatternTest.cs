using System;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
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
		public void CtorProtection()
		{
			var orm = new Mock<IDomainInspector>();

			Executing.This(() => new OneToOneUnidirectionalToManyToOnePattern(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new OneToOneUnidirectionalToManyToOnePattern(null, new ExplicitDeclarationsHolder())).Should().Throw<ArgumentNullException>();
			Executing.This(() => new OneToOneUnidirectionalToManyToOnePattern(orm.Object, null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenDeclaredExplicitOneToOneAndIsUnirectionalThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.IsAny<Type>())).Returns(true);

			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof (MyClass), typeof (MyOneClass));
			edo.OneToOneRelations.Add(relation);
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(orm.Object, edo);
			pattern.Match(relation).Should().Be.True();
		}

		[Test]
		public void WhenDeclaredExplicitOneToOneAndIsNotUnirectionalThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.IsAny<Type>())).Returns(true);

			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneBidirectional));
			edo.OneToOneRelations.Add(relation);
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(orm.Object, edo);
			pattern.Match(relation).Should().Be.False();
		}

		[Test]
		public void WhenNotDeclaredExplicitOneToOneThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.IsAny<Type>())).Returns(true);

			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneClass));
			var pattern = new OneToOneUnidirectionalToManyToOnePattern(orm.Object, edo);
			pattern.Match(relation).Should().Be.False();
		}

		[Test]
		public void WhenToSideIsNotAnEntityThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MyClass))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(MyOneClass))).Returns(false);

			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneClass));
			edo.OneToOneRelations.Add(relation);

			var pattern = new OneToOneUnidirectionalToManyToOnePattern(orm.Object, edo);
			pattern.Match(relation).Should().Be.False();
		}

		[Test]
		public void WhenFromSideIsNotAnEntityThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MyClass))).Returns(false);
			orm.Setup(x => x.IsEntity(typeof(MyOneClass))).Returns(true);

			var edo = new ExplicitDeclarationsHolder();
			var relation = new Relation(typeof(MyClass), typeof(MyOneClass));
			edo.OneToOneRelations.Add(relation);

			var pattern = new OneToOneUnidirectionalToManyToOnePattern(orm.Object, edo);
			pattern.Match(relation).Should().Be.False();
		}
	}
}