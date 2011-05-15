using System.Collections.Generic;
using ConfOrm.Patterns;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ComponentMultiUsagePatternTest
	{
		private class MyClassDoubleUsage
		{
			public MyComponent Component1 { get; set; }
			public MyComponent Component2 { get; set; }
		}

		private class MyComponent
		{
			public string Prop1 { get; set; }
			public ICollection<string> Prop2 { get; set; }
		}

		private class MyClassSingleUsage
		{
			public MyComponent Component { get; set; }
		}

		private class MyClass
		{
			public string Fake1 { get; set; }
			public string Fake2 { get; set; }
		}

		[Test]
		public void WhenDoubleUsageThenMatch()
		{
			var basePath1 = new PropertyPath(null, typeof (MyClassDoubleUsage).GetProperty("Component1"));
			var basePath2 = new PropertyPath(null, typeof(MyClassDoubleUsage).GetProperty("Component2"));
			var prop1OfComponent = typeof(MyComponent).GetProperty("Prop1");
			var prop2OfComponent = typeof(MyComponent).GetProperty("Prop2");
			var pattern = new ComponentMultiUsagePattern();
			pattern.Match(new PropertyPath(basePath1, prop1OfComponent)).Should().Be.True();
			pattern.Match(new PropertyPath(basePath1, prop2OfComponent)).Should().Be.True();
			pattern.Match(new PropertyPath(basePath2, prop1OfComponent)).Should().Be.True();
			pattern.Match(new PropertyPath(basePath2, prop2OfComponent)).Should().Be.True();
		}

		[Test]
		public void WhenSingleUsageThenNoMatch()
		{
			var basePath = new PropertyPath(null, typeof(MyClassSingleUsage).GetProperty("Component"));
			var prop1OfComponent = typeof(MyComponent).GetProperty("Prop1");
			var prop2OfComponent = typeof(MyComponent).GetProperty("Prop2");
			var pattern = new ComponentMultiUsagePattern();
			pattern.Match(new PropertyPath(basePath, prop1OfComponent)).Should().Be.False();
			pattern.Match(new PropertyPath(basePath, prop2OfComponent)).Should().Be.False();
		}

		[Test]
		public void DoesNotMatchAtAnyFirstLevel()
		{
			var pattern = new ComponentMultiUsagePattern();
			pattern.Match(new PropertyPath(null, typeof(MyClass).GetProperty("Fake1"))).Should().Be.False();
			pattern.Match(new PropertyPath(null, typeof(MyClass).GetProperty("Fake2"))).Should().Be.False();
			pattern.Match(new PropertyPath(null, typeof(MyClassDoubleUsage).GetProperty("Component1"))).Should().Be.False();
			pattern.Match(new PropertyPath(null, typeof(MyClassDoubleUsage).GetProperty("Component2"))).Should().Be.False();
		}
	}
}