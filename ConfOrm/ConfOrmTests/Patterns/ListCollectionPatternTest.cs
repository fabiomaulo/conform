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
	public class ListCollectionPatternTest
	{
		private class Entity
		{
			private ICollection<string> others;
			private IList<string> emails;
			public IList<string> NickNames { get; set; }
			public byte[] Bytes { get; set; }
			public string StringProp { get; set; }

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
		public void CtorProtection()
		{
			Executing.This(() => new ListCollectionPattern(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void MatchWithListProperty()
		{
			var orm = new Mock<IDomainInspector>();

			var mi = typeof(Entity).GetProperty("NickNames");
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithListField()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndListField()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetProperty("Emails");
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetProperty("Others");
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetProperty("StringProp");
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var orm = new Mock<IDomainInspector>();
			var mi = typeof(Entity).GetProperty("Bytes");
			var p = new ListCollectionPattern(orm.Object);
			p.Match(mi).Should().Be.False();
		}
	}
}