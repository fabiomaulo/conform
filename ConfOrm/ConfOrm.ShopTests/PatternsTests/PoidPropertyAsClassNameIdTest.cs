using ConfOrm.Shop.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.PatternsTests
{
	public class PoidPropertyAsClassNameIdTest
	{
		private class MyClass
		{
			public int MyClassId { get; set; }
			public int myClassId { get; set; }
			public int Id { get; set; }
		}

		[Test]
		public void WhenNullNotThrow()
		{
			var pattern = new PoidPropertyAsClassNameId();
			ActionAssert.NotThrow(()=> pattern.Match(null));
		}

		[Test]
		public void MatchWithMyClassIdProperty()
		{
			var pattern = new PoidPropertyAsClassNameId();
			pattern.Match(ForClass<MyClass>.Property(x => x.MyClassId)).Should().Be.True();
		}

		[Test]
		public void NoMatchWith_myClassId_Property()
		{
			var pattern = new PoidPropertyAsClassNameId();
			pattern.Match(ForClass<MyClass>.Property(x => x.myClassId)).Should().Be.False();
		}

		[Test]
		public void NoMatchWithMyClassIdProperty()
		{
			var pattern = new PoidPropertyAsClassNameId();
			pattern.Match(ForClass<MyClass>.Property(x => x.Id)).Should().Be.False();
		}
	}
}