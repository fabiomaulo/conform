using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalManyToManyPatternTest
	{
		private class User
		{
			public IEnumerable<Role> Roles { get; set; }
		}

		private class Role
		{
			public IEnumerable<User> Users { get; set; }
		}

		private class UserMap
		{
			public IDictionary<string, RoleMap> RolesMap { get; set; }
		}

		private class UserMix
		{
			public IDictionary<string, RoleMix> RolesMap { get; set; }
		}

		private class RoleMix
		{
			public IEnumerable<UserMix> Users { get; set; }
		}

		private class RoleMap
		{
			public IDictionary<UserMap, string> UsersMap { get; set; }
		}

		private class Parent
		{
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public Parent Parent { get; set; }
		}

		[Test]
		public void MatchManyToManyInPlainCollection()
		{
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(User).GetProperty("Roles")).Should().Be.True();
			pattern.Match(typeof(Role).GetProperty("Users")).Should().Be.True();
		}

		[Test]
		public void MatchManyToManyInDictionaryValue()
		{
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(UserMap).GetProperty("RolesMap")).Should().Be.True();
		}

		[Test]
		public void MatchManyToManyInDictionaryKey()
		{
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(RoleMap).GetProperty("UsersMap")).Should().Be.True();
		}

		[Test]
		public void MatchManyToManyWithMixCollectionAndDictionary()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(UserMix).GetProperty("RolesMap")).Should().Be.True();
			pattern.Match(typeof(RoleMix).GetProperty("Users")).Should().Be.True();
		}

		[Test]
		public void NoMatchOneToMany()
		{
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(Parent).GetProperty("Children")).Should().Be.False();
		}

		[Test]
		public void NoMatchManyToOne()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new BidirectionalManyToManyPattern();
			pattern.Match(typeof(Child).GetProperty("Parent")).Should().Be.False();
		}
	}
}