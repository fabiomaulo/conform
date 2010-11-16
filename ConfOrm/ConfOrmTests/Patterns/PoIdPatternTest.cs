using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PoIdPatternTest
	{
		private class TestEntity
		{
			public int Id { get; set; }
			public int id { get; set; }
			public int PoId { get; set; }
			public int POID { get; set; }
			public int OId { get; set; }
			public int Something { get; set; }
		}

		private class MyClass
		{
			public int MyClassId { get; set; }
			public int myClassId { get; set; }
		}

		[Test]
		public void PatternMatch()
		{
			var pattern = new PoIdPattern();
			PropertyInfo pi = typeof(TestEntity).GetProperty("Id");
			pi.Satisfy(p=> pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("id");
			pi.Satisfy(p => pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("PoId");
			pi.Satisfy(p => pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("POID");
			pi.Satisfy(p => pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("OId");
			pi.Satisfy(p => pattern.Match(p));
			pi = typeof(TestEntity).GetProperty("Something");
			pi.Satisfy(p => !pattern.Match(p));
		}
		[Test]
		public void MatchWithMyClassIdProperty()
		{
			var pattern = new PoIdPattern();
			pattern.Match(ForClass<MyClass>.Property(x => x.MyClassId)).Should().Be.True();
		}

		[Test]
		public void NoMatchWith_myClassId_Property()
		{
			var pattern = new PoIdPattern();
			pattern.Match(ForClass<MyClass>.Property(x => x.myClassId)).Should().Be.False();
		}

	}
}