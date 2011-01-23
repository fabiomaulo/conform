using System;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PropertyPathExtensionsInversePropertyPath
	{
		private class MyClass
		{
			public ComponentLevel0 Component { get; set; }
		}

		private class ComponentLevel0
		{
			private ComponentLevel1 componentLevel1;
			public string Something0 { get; set; }

			public ComponentLevel1 ComponentLevel1
			{
				get { return componentLevel1; }
			}
		}

		private class ComponentLevel1
		{
			private ComponentLevel2 componentLevel2;
			public string Something1 { get; set; }

			public ComponentLevel2 ComponentLevel2
			{
				get { return componentLevel2; }
			}
		}

		private class ComponentLevel2
		{
			public string Something2 { get; set; }
		}

		[Test]
		public void WhenNullThenThrows()
		{
			Executing.This(() => ((PropertyPath)null).InverseProgressivePath().ToList()).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenOneLevelThenPropertyPathItself()
		{
			var path0 = new PropertyPath(null, ForClass<MyClass>.Property(x => x.Component));
			var actual = path0.InverseProgressivePath().ToList();
			actual.Count.Should().Be(1);
			actual.Single().Should().Be(path0);
		}

		[Test]
		public void WhenTwoLevelsThen2PropertyPaths()
		{
			// x.Component.Something0
			var path0 = new PropertyPath(null, ForClass<MyClass>.Property(x => x.Component));
			var path1 = new PropertyPath(path0, ForClass<ComponentLevel0>.Property(x => x.Something0));
			var actual = path1.InverseProgressivePath().ToList();
			actual.Count.Should().Be(2);

			var expected1 = new PropertyPath(null, ForClass<ComponentLevel0>.Property(x => x.Something0));
			var expected2 = path1;
			actual.First().Should().Be(expected1);
			actual.Skip(1).First().Should().Be(expected2);
		}

		[Test]
		public void WhenThreeLevelsThen3PropertyPaths()
		{
			// x.Component.ComponentLevel1.Something1
			var path0 = new PropertyPath(null, ForClass<MyClass>.Property(x => x.Component));
			var path1 = new PropertyPath(path0, ForClass<ComponentLevel0>.Property(x => x.ComponentLevel1));
			var path2 = new PropertyPath(path1, ForClass<ComponentLevel1>.Property(x => x.Something1));
			var actual = path2.InverseProgressivePath().ToList();
			actual.Count.Should().Be(3);

			var expected1 = new PropertyPath(null, ForClass<ComponentLevel1>.Property(x => x.Something1));
			var expected2 = new PropertyPath(new PropertyPath(null, ForClass<ComponentLevel0>.Property(x => x.ComponentLevel1)), ForClass<ComponentLevel1>.Property(x => x.Something1));
			var expected3 = path2;
			actual.First().Should().Be(expected1);
			actual.Skip(1).First().Should().Be(expected2);
			actual.Skip(2).First().Should().Be(expected3);
		}

		[Test]
		public void WhenFourLevelsThen4PropertyPaths()
		{
			// x.Component.ComponentLevel1.ComponentLevel2.Something2
			var path0 = new PropertyPath(null, ForClass<MyClass>.Property(x => x.Component));
			var path1 = new PropertyPath(path0, ForClass<ComponentLevel0>.Property(x => x.ComponentLevel1));
			var path2 = new PropertyPath(path1, ForClass<ComponentLevel1>.Property(x => x.ComponentLevel2));
			var path3 = new PropertyPath(path2, ForClass<ComponentLevel2>.Property(x => x.Something2));
			var actual = path3.InverseProgressivePath().ToList();
			actual.Count.Should().Be(4);

			var expected1 = new PropertyPath(null, ForClass<ComponentLevel2>.Property(x => x.Something2));
			var expected2 = new PropertyPath(new PropertyPath(null, ForClass<ComponentLevel1>.Property(x => x.ComponentLevel2)), ForClass<ComponentLevel2>.Property(x => x.Something2));
			var expected3 = new PropertyPath(new PropertyPath(new PropertyPath(null, ForClass<ComponentLevel0>.Property(x => x.ComponentLevel1)), ForClass<ComponentLevel1>.Property(x => x.ComponentLevel2)), ForClass<ComponentLevel2>.Property(x => x.Something2));
			var expected4 = path3;
			actual.First().Should().Be(expected1);
			actual.Skip(1).First().Should().Be(expected2);
			actual.Skip(2).First().Should().Be(expected3);
			actual.Skip(3).First().Should().Be(expected4);
		}
	}
}