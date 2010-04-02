using System;
using System.Collections.Generic;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToManyPatternTest
	{
		private class MyClass
		{
			public ICollection<MyOtherClass> MyOtherClasses { get; set; }
			public ICollection<MyBidirect> MyBidirects { get; set; }
			public IDictionary<MyOtherClass, string> MapKey { get; set; }
			public IDictionary<string, MyOtherClass> MapValue { get; set; }
		}

		private class MyOtherClass
		{
			
		}

		private class MyBidirect
		{
			public ICollection<MyClass> MyClasses { get; set; }
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyUnidirectionalThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyOtherClass)))).Returns(true);

			var pattern = new ManyToManyPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(x => x.MyOtherClasses)).Should().Be.True();
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryKeyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyOtherClass)))).Returns(true);

			var pattern = new ManyToManyPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(x => x.MapKey)).Should().Be.True();
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryValueThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyOtherClass)))).Returns(true);

			var pattern = new ManyToManyPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(x => x.MapValue)).Should().Be.True();
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyBidirectionalThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyPattern(orm.Object);

			pattern.Match(ForClass<MyClass>.Property(x => x.MyBidirects)).Should().Be.True();
			pattern.Match(ForClass<MyBidirect>.Property(x => x.MyClasses)).Should().Be.True();
		}

		[Test]
		public void WhenRelationNotDeclaredAsManyToManyUnidirectionalThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new ManyToManyPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(x => x.MyOtherClasses)).Should().Be.False();
			pattern.Match(ForClass<MyClass>.Property(x => x.MyBidirects)).Should().Be.False();
			pattern.Match(ForClass<MyBidirect>.Property(x => x.MyClasses)).Should().Be.False();
		}
	}
}