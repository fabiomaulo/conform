using System.Reflection;
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
	}
}