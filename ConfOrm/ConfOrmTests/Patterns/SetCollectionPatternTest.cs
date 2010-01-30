using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;
using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class SetCollectionPatternTest
	{
		private class EntityWithSets
		{
			private ICollection<string> others;
			private ISet<string> emails;
			public ISet<string> NickNames { get; set; }

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
		public void MatchWithSetProperty()
		{
			var mi = typeof (EntityWithSets).GetProperty("NickNames");
			var p = new SetCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithSetField()
		{
			var mi = typeof(EntityWithSets).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new SetCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndSetField()
		{
			var mi = typeof(EntityWithSets).GetProperty("Emails");
			var p = new SetCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(EntityWithSets).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new SetCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(EntityWithSets).GetProperty("Others");
			var p = new SetCollectionPattern();
			p.Match(mi).Should().Be.False();
		}
	}
}