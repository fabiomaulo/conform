using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
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

		private Mock<IDomainInspector> GetOrm()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name.ToLowerInvariant() == "id"))).Returns(true);
			return orm;
		}

		[Test]
		public void ClassWithoutPoidIsComponent()
		{
			var orm = GetOrm();
			var p = new ComponentPattern(orm.Object);
			p.Match(typeof (AComponent)).Should().Be.True();
		}

		[Test]
		public void ClassWithPoidIsNotComponent()
		{
			var orm = GetOrm();
			var p = new ComponentPattern(orm.Object);
			p.Match(typeof(AEntity)).Should().Be.False();
		}

		[Test]
		public void ClassWithPoidFieldIsNotComponent()
		{
			var orm = GetOrm();
			var p = new ComponentPattern(orm.Object);
			p.Match(typeof(Entity)).Should().Be.False();
		}

		[Test]
		public void EnumIsNotComponent()
		{
			var orm = GetOrm();
			var p = new ComponentPattern(orm.Object);
			p.Match(typeof(Something)).Should().Be.False();
		}
	}
}