using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalOneToManyMemberPatternTest
	{
		private MemberInfo parentChildren = typeof(Parent).GetProperty("Children");
		private MemberInfo childParent = typeof(Child).GetProperty("Parent");
		private MemberInfo humanValueFriends = typeof(HumanValue).GetProperty("Friends");
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
			public IDictionary<string, HumanValue> Friends { get; set; }
		}

		[Test]
		public void MatchOneToMany()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);

			var pattern = new BidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(parentChildren).Should().Be.True();
		}

		[Test]
		public void NoMatchManyToOne()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(childParent).Should().Be.False();
		}

		[Test]
		public void NoMatchManyToMany()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			orm.Setup(o => o.IsManyToMany(It.IsAny<Type>(), It.IsAny<Type>())).Returns(true);
			var pattern = new BidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(childParent).Should().Be.False();
			pattern.Match(parentChildren).Should().Be.False();
		}

		[Test]
		public void NoMatchManyToManyInDictionaryValue()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			orm.Setup(o => o.IsManyToMany(It.IsAny<Type>(), It.IsAny<Type>())).Returns(true);
			var pattern = new BidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(humanValueFriends).Should().Be.False();
		}

		[Test]
		public void MatchOneToManyInDictionaryValue()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new BidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(humanValueFriends).Should().Be.False();
		}

	}
}