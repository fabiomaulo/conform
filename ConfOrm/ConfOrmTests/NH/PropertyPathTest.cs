using System;
using System.Reflection;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PropertyPathTest
	{
		PropertyInfo myClassComponent1 = typeof(MyClass).GetProperty("Component1");
		PropertyInfo myClassComponent2 = typeof(MyClass).GetProperty("Component2");
		PropertyInfo myComponentComponent = typeof(MyComponent).GetProperty("Component");
		PropertyInfo myComponentInComponentName = typeof(MyComponentInComponent).GetProperty("Name");

		private class MyClass
		{
			public MyComponent Component1 { get; set; }
			public MyComponent Component2 { get; set; }
		}

		private class MyComponent
		{
			public MyComponentInComponent Component { get; set; }
		}

		private class MyComponentInComponent
		{
			public string Name { get; set; }
		}

		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new PropertyPath(null, null));
			ActionAssert.NotThrow(() => new PropertyPath(null, myClassComponent1));
			ActionAssert.Throws<ArgumentNullException>(() => new PropertyPath(new PropertyPath(null, myClassComponent1), null));
		}

		[Test]
		public void HashCodeLevel1()
		{
			new PropertyPath(null, myClassComponent1).GetHashCode().Should().Be(new PropertyPath(null, myClassComponent1).GetHashCode());
			new PropertyPath(null, myClassComponent2).GetHashCode().Should().Not.Be(new PropertyPath(null, myClassComponent1).GetHashCode());
		}

		[Test]
		public void HashCodeLevel2()
		{
			var myClassComponent1Path = new PropertyPath(null, myClassComponent1);
			var myClassComponent2Path = new PropertyPath(null, myClassComponent2);
			new PropertyPath(myClassComponent1Path, myComponentComponent).GetHashCode().Should().Be(new PropertyPath(myClassComponent1Path, myComponentComponent).GetHashCode());
			new PropertyPath(myClassComponent2Path, myComponentComponent).GetHashCode().Should().Not.Be(new PropertyPath(myClassComponent1Path, myComponentComponent).GetHashCode());
		}

		[Test]
		public void HashCodeLevel3()
		{
			var myClassComponent1Path = new PropertyPath(null, myClassComponent1);
			var myClassComponent2Path = new PropertyPath(null, myClassComponent2);
			var myClassComponent1ComponentPath = new PropertyPath(myClassComponent1Path, myComponentComponent);
			var myClassComponent2ComponentPath = new PropertyPath(myClassComponent2Path, myComponentComponent);
			new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName).GetHashCode().Should().Be(new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName).GetHashCode());
			new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName).GetHashCode().Should().Not.Be(new PropertyPath(myClassComponent2ComponentPath, myComponentInComponentName).GetHashCode());
		}

		[Test]
		public void EqualsLevel1()
		{
			new PropertyPath(null, myClassComponent1).Should().Be(new PropertyPath(null, myClassComponent1));
			new PropertyPath(null, myClassComponent2).Should().Not.Be(new PropertyPath(null, myClassComponent1));
		}

		[Test]
		public void EqualsLevel2()
		{
			var myClassComponent1Path = new PropertyPath(null, myClassComponent1);
			var myClassComponent2Path = new PropertyPath(null, myClassComponent2);
			new PropertyPath(myClassComponent1Path, myComponentComponent).Should().Be(new PropertyPath(myClassComponent1Path, myComponentComponent));
			new PropertyPath(myClassComponent2Path, myComponentComponent).Should().Not.Be(new PropertyPath(myClassComponent1Path, myComponentComponent));
		}

		[Test]
		public void EqualsLevel3()
		{
			var myClassComponent1Path = new PropertyPath(null, myClassComponent1);
			var myClassComponent2Path = new PropertyPath(null, myClassComponent2);
			var myClassComponent1ComponentPath = new PropertyPath(myClassComponent1Path, myComponentComponent);
			var myClassComponent2ComponentPath = new PropertyPath(myClassComponent2Path, myComponentComponent);
			new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName).Should().Be(new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName));
			new PropertyPath(myClassComponent1ComponentPath, myComponentInComponentName).Should().Not.Be(new PropertyPath(myClassComponent2ComponentPath, myComponentInComponentName));
		}
	}
}