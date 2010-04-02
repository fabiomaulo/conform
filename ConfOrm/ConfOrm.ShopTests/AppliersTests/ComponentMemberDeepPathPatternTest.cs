using System.Collections.Generic;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ComponentMemberDeepPathPatternTest
	{
		private class MyClassWithComponent
		{
			public MyComponent Component1 { get; set; }
		}

		private class MyComponent
		{
			public string Prop1 { get; set; }
			public ICollection<string> Prop2 { get; set; }
			public MyClass MyNested { get; set; }
			public ICollection<MyClass> Collection { get; set; }
			public MyOtherEntity ManyToOne { get; set; }
			public MyOtherEntity OneToOne { get; set; }
			public object Any { get; set; }
		}

		private class MyClass
		{
			public string Fake1 { get; set; }
		}

		private class MyOtherEntity
		{
			public string Name { get; set; }
		}

		[Test]
		public void WhenPropertyAtFirstLevelThenDoesNotMatch()
		{
			var pattern = new ComponentMemberDeepPathPattern();
			pattern.Match(new PropertyPath(null, ForClass<MyClass>.Property(x=>x.Fake1))).Should().Be.False();
			pattern.Match(new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x=> x.Component1))).Should().Be.False();
		}

		[Test]
		public void WhenPreviousLevelIsCollectionThenDoesNotMatch()
		{
			var lelev0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Component1));
			var lelev1 = new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.Collection));
			var lelev2 = new PropertyPath(lelev1, ForClass<MyClass>.Property(x => x.Fake1));
			var pattern = new ComponentMemberDeepPathPattern();
			pattern.Match(lelev2).Should().Be.False();
		}

		[Test]
		public void WhenLevel1ThenMatch()
		{
			var lelev0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Component1));
			var deppLevels = new[]
			                 	{
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.Prop1)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.Prop2)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.MyNested)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.Collection)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.ManyToOne)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.OneToOne)),
			                 		new PropertyPath(lelev0, ForClass<MyComponent>.Property(x => x.Any)),
			                 	};
			var pattern = new ComponentMemberDeepPathPattern();
			deppLevels.All(pp => pp.Satisfy(x => pattern.Match(x)));
		}
	}
}