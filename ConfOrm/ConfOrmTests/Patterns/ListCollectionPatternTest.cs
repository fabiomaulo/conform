using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ListCollectionPatternTest
	{
		private class Entity
		{
			private ICollection<string> others;
			private IList<string> emails;
			public IList<string> NickNames { get; set; }

			public ICollection<string> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
		}

		[Test]
		public void MatchWithListProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var p = new ListCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithListField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ListCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndListField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var p = new ListCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ListCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var p = new ListCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

	}
}