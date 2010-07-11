using System;
using System.Collections;
using System.Collections.Generic;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class CollectionOfComponentsPatternTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<MyComponent> Components { get; set; }
			public ICollection<string > Elements { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<MyComponent> Components { get; set; }
			public ICollection<string> Elements { get; set; }			
		}

		private class MyRelated
		{
			public int Id { get; set; }			
		}

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(p => p.Something)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsManyToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyComponent>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsElementsThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);

			pattern.Match(ForClass<MyClass>.Property(p => p.Elements)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsElementsInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);

			pattern.Match(ForClass<MyComponent>.Property(p => p.Elements)).Should().Be.False();
		}

		[Test]
		public void WhenComponentPropertyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Component)).Should().Be.False();
		}

		[Test]
		public void WhenComponentsCollectionThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Components)).Should().Be.True();
		}

		[Test]
		public void WhenComponentsCollectionInsideComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPattern(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			pattern.Match(ForClass<MyComponent>.Property(p => p.Components)).Should().Be.True();
		}
	}
}