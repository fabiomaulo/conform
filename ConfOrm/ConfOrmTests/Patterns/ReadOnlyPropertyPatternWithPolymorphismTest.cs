using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ReadOnlyPropertyPatternWithPolymorphismTest
	{
		internal const BindingFlags RootClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		private class BaseEntity
		{
			public int Id { get; set; }
			public bool IsTransient { get { return false; } }
	}

		private class Entity : BaseEntity
		{
			public bool IsValid { get { return false; } }
			public string Something { get; set; }
		}

		private class Person : Entity
		{
			public string Name { get; set; }
		}

		[Test]
		public void WhenReadOnlyDeclaredAtFirstLevelThenMatch()
		{
			var pattern = new ReadOnlyPropertyPattern();
			var prop = typeof(Person).GetProperty("IsTransient", RootClassPropertiesBindingFlags);
			pattern.Match(prop).Should().Be.True();
		}

		[Test]
		public void WhenReadOnlyAtSecondLevelThenMatch()
		{
			var pattern = new ReadOnlyPropertyPattern();
			var prop = typeof(Person).GetProperty("IsValid", RootClassPropertiesBindingFlags);
			pattern.Match(prop).Should().Be.True();
		}
	}
}