using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ComponentPatternTest
	{
		// a class without Poid is a Component
		private class AComponent
		{
			public string S { get; set; }
		}
		private class AEntity
		{
			public int Id { get; set; }
		}
		private class Entity
		{
			private int id;
		}

		private enum Something
		{
			
		}

		[Test]
		public void ClassWithoutPoidIsComponent()
		{
			var p = new ComponentPattern();
			p.Match(typeof (AComponent)).Should().Be.True();
		}

		[Test]
		public void ClassWithPoidIsNotComponent()
		{
			var p = new ComponentPattern();
			p.Match(typeof(AEntity)).Should().Be.False();
		}

		[Test]
		public void ClassWithPoidFieldIsNotComponent()
		{
			var p = new ComponentPattern();
			p.Match(typeof(Entity)).Should().Be.False();
		}

		[Test]
		public void EnumIsNotComponent()
		{
			var p = new ComponentPattern();
			p.Match(typeof(Something)).Should().Be.False();
		}
	}
}