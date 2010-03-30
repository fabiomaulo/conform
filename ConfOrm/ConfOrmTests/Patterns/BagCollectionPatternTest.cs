using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BagCollectionPatternTest
	{
		// match any IEnumerable
		private class Entity
		{
			private ICollection<string> emails;
			public IEnumerable<string> NickNames { get; set; }
			public byte[] Bytes { get; set; }
			public object Emails
			{
				get { return emails; }
			}

			public string Simple { get; set; }
		}

		[Test]
		public void MatchWithEnumerableProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var p = new BagCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithEnumerableField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new BagCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithObjectPropertyAndEnumerableField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var p = new BagCollectionPattern();
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var mi = typeof(Entity).GetProperty("Simple");
			var p = new BagCollectionPattern();
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("Bytes");
			var p = new BagCollectionPattern();
			p.Match(mi).Should().Be.False();
		}
	}
}