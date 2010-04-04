using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToManyInCollectionTableApplierTest
	{
		private class MyClass
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
			public IDictionary<MyBidirect, string> MapKey { get; set; }
			public IDictionary<string, MyBidirect> MapValue { get; set; }
			public MyComponent MyComponent { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
		}

		private class MyBidirect
		{
			public ICollection<MyClass> MyClasses { get; set; }
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryKeyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MapKey));

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryValueThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MapValue));

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenManyToManyCollectionThenApplyTableFromMasterEntityToNoMaster()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(bipath, bicollectionMapper.Object);

			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}

		[Test]
		public void WhenManyToManyCollectionWithOutMasterThenApplyTableAlphabetical()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(bipath, bicollectionMapper.Object);

			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));
		}

		[Test]
		public void WhenManyToManyCollectionInsideComponentThenApplyFromEntityToEntity()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);

			var pathEntity = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyComponent));
			var path = new PropertyPath(pathEntity, ForClass<MyComponent>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}
	}
}