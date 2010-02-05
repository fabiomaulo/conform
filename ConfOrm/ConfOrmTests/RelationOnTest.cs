using System;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class RelationOnTest
	{
		private MemberInfo b1Property = typeof (A).GetProperty("B1");
		private MemberInfo b2Property = typeof(A).GetProperty("B2");

		private class A
		{
			public B B1 { get; set; }
			public B B2 { get; set; }
		}
		private class B
		{
			
		}

		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new RelationOn(null, null, null));
			ActionAssert.Throws<ArgumentNullException>(() => new RelationOn(typeof(A), null, null));
			ActionAssert.Throws<ArgumentNullException>(() => new RelationOn(null, null, typeof(A)));
			ActionAssert.Throws<ArgumentNullException>(() => new RelationOn(typeof(A), null, typeof(A)));
		}

		[Test]
		public void Equals()
		{
			(new RelationOn(typeof(A), b1Property, typeof(B))).Should().Be.EqualTo(new RelationOn(typeof(A), b1Property, typeof(B)));
			(new RelationOn(typeof(A), b1Property, typeof(B))).Should().Not.Be.EqualTo(new RelationOn(typeof(A), b2Property, typeof(B)));
			(new RelationOn(typeof(A), b1Property, typeof(B))).Should().Not.Be.EqualTo(new Relation(typeof(A), typeof(B)));
			(new RelationOn(typeof(B), b1Property, typeof(A))).Should().Not.Be.EqualTo(new object());
		}

		[Test]
		public void Unicity()
		{
			(new RelationOn(typeof(A), b1Property, typeof(B))).GetHashCode().Should().Be.EqualTo(new RelationOn(typeof(A), b1Property, typeof(B)).GetHashCode());
			(new RelationOn(typeof(A), b1Property, typeof(B))).GetHashCode().Should().Not.Be.EqualTo(new RelationOn(typeof(A), b2Property, typeof(B)).GetHashCode());
		}

		[Test]
		public void FriendlyToString()
		{
			var friendly = (new RelationOn(typeof (A), b1Property, typeof (B))).ToString();
			friendly.Should().Contain(typeof (A).FullName).And.Contain(typeof (B).FullName).And.Contain("B1");
		}
	}
}