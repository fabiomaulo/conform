using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class DictionaryCollectionPatternTest
	{
		private class Entity
		{
			private ICollection<string> others;
			private IDictionary<string, string> emails;
			public IDictionary<string, string> NickNames { get; set; }

			public ICollection<KeyValuePair<string,string>> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
		}

		[Test]
		public void MatchWithDictionaryProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var p = new DictionaryCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithDictionaryField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new DictionaryCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndDictionaryField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var p = new DictionaryCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new DictionaryCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var p = new DictionaryCollectionPattern();
			p.Match(mi).Should().Be.False();
		}
	}
}