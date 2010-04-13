using System;
using System.Collections;
using System.Collections.Generic;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class CollectionOfElementsPatternTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public IDictionary<string, MyRelated> MapRelationOnValue { get; set; }
			public IDictionary<MyRelated, string> MapRelationOnKey { get; set; }
			public IDictionary<string, string> MapOfStrings { get; set; }
			public ICollection<string> Strings { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<string> Strings { get; set; }
		}

		private class MyRelated
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(p => p.Something)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsManyToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyComponent>.Property(p => p.Relateds)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.MapRelationOnValue)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapKeyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.MapRelationOnKey)).Should().Be.False();
		}

		[Test]
		public void WhenNoRelationBetweenEntitiesThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);

			pattern.Match(ForClass<MyClass>.Property(p => p.Strings)).Should().Be.True();
			pattern.Match(ForClass<MyClass>.Property(p => p.MapOfStrings)).Should().Be.True();
		}

		[Test]
		public void WhenNoRelationWithComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsPattern(orm.Object);

			pattern.Match(ForClass<MyComponent>.Property(p => p.Strings)).Should().Be.True();
		}
	}
}