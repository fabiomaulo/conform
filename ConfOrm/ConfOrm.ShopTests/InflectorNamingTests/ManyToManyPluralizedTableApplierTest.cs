using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorNamingTests
{
	public class ManyToManyPluralizedTableApplierTest
	{
		private class MyClass
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
		}

		private class MyBidirect
		{
			public ICollection<MyClass> MyClasses { get; set; }
		}

		[Test]
		public void WhenManyToManyCollectionWithOutMasterThenApplyTableAlphabetical()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			var inflector = new Mock<IInflector>();

			var pattern = new ManyToManyPluralizedTableApplier(orm.Object, inflector.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			inflector.Verify(x => x.Pluralize("MyBidirect"));
			inflector.Verify(x => x.Pluralize("MyClass"));
		}
	}
}