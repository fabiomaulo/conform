using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyPatternTest
	{
		private class Parent
		{
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public Parent Parent { get; set; }
		}

		private class HumanValue
		{
			public IDictionary<string,HumanValue> Friends { get; set; }
		}

		[Test]
		public void MatchOneToMany()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalOneToManyPattern(orm.Object);
			pattern.Match(new Relation(typeof (Parent), typeof (Child))).Should().Be.True();
		}

		[Test]
		public void NoMatchManyToOne()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalOneToManyPattern(orm.Object);
			pattern.Match(new Relation(typeof(Child), typeof(Parent))).Should().Be.False();
		}

		[Test]
		public void NoMatchManyToMany()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(o => o.IsManyToMany(It.IsAny<Type>(), It.IsAny<Type>())).Returns(true);
			var pattern = new BidirectionalOneToManyPattern(orm.Object);
			pattern.Match(new Relation(typeof(Child), typeof(Parent))).Should().Be.False();
			pattern.Match(new Relation(typeof(Parent), typeof(Child))).Should().Be.False();
		}

		[Test]
		public void NoMatchManyToManyInDictionaryValue()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(o => o.IsManyToMany(It.IsAny<Type>(), It.IsAny<Type>())).Returns(true);
			var pattern = new BidirectionalOneToManyPattern(orm.Object);
			pattern.Match(new Relation(typeof(HumanValue), typeof(HumanValue))).Should().Be.False();
		}

		[Test]
		public void MatchOneToManyInDictionaryValue()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalOneToManyPattern(orm.Object);
			pattern.Match(new Relation(typeof(HumanValue), typeof(HumanValue))).Should().Be.False();
		}
	}
}