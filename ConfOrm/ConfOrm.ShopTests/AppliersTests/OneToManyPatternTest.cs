using System;
using System.Collections;
using System.Collections.Generic;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class OneToManyPatternTest
	{
		private class MyClass
		{
			public ICollection<Element> Elements { get; set; }
			public IList Something { get; set; }
			public IDictionary<string, Element> DicElements { get; set; }
		}

		private class Element { }

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyPattern(orm.Object);

			pattern.Match(ForClass<MyClass>.Property(p => p.Something)).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof (MyClass)), It.Is<Type>(t => t == typeof (Element)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.Elements)).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(Element)))).Returns(true);

			pattern.Match(ForClass<MyClass>.Property(p => p.DicElements)).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsNotOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyPattern(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(Element)))).Returns(false);

			pattern.Match(ForClass<MyClass>.Property(p => p.Elements)).Should().Be.False();
		}
	}
}