using System;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class RelationTest
	{
		private class A { }
		private class B { }

		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new Relation(null, null));
			ActionAssert.Throws<ArgumentNullException>(() => new Relation(typeof(A), null));
			ActionAssert.Throws<ArgumentNullException>(() => new Relation(null, typeof(A)));
		}

		[Test]
		public void Equals()
		{
			(new Relation(typeof(A), typeof(B))).Should().Be.EqualTo(new Relation(typeof(A), typeof(B)));
			(new Relation(typeof(B), typeof(A))).Should().Not.Be.EqualTo(new Relation(typeof(A), typeof(B)));
			(new Relation(typeof(B), typeof(A))).Should().Not.Be.EqualTo(new object());
		}

		[Test]
		public void Unicity()
		{
			(new Relation(typeof(A), typeof(B))).GetHashCode().Should().Be.EqualTo(new Relation(typeof(A), typeof(B)).GetHashCode());
			(new Relation(typeof(B), typeof(A))).GetHashCode().Should().Not.Be.EqualTo(new Relation(typeof(A), typeof(B)).GetHashCode());
		}

		[Test]
		public void FriendlyToString()
		{
			var friendly = (new Relation(typeof(A), typeof(B))).ToString();
			friendly.Should().Contain(typeof(A).FullName).And.Contain(typeof(B).FullName);
		}
	}
}