using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToManyTableApplierTest
	{
		private class MyClass
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
			public IDictionary<MyBidirect, string> MapKey { get; set; }
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
		public void WhenManyToManyCollectionThenApplyMasterEntityClassToOtherSide()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			pattern.Match(path).Should().Be.True();
			
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}

		[Test]
		public void WhenManyToManyCollectionInsideComponentThenApplyEntityClassToOtherSide()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);

			var pathEntity = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyComponent));
			var path = new PropertyPath(pathEntity, ForClass<MyComponent>.Property(x => x.MyBidirects));
			pattern.Match(path).Should().Be.True();

			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}

		[Test]
		public void WhenManyToManyDictionaryThenApplyApplyEntityClassToOtherSide()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MapKey));
			pattern.Match(path).Should().Be.True();

			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}

		[Test]
		public void WhenManyToManyBidirectionalThenApplyMasterEntityClassToOtherSideInBoth()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			pattern.Match(path).Should().Be.True();
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(path, collectionMapper.Object);
			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			pattern.Match(bipath).Should().Be.True();
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(bipath, bicollectionMapper.Object);
			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyClassToMyBidirect")));
		}

		[Test]
		public void WhenManyToManyBidirectionalBothMasterThenApplyAlphabeticalInBoth()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			pattern.Match(path).Should().Be.True();
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(path, collectionMapper.Object);
			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			pattern.Match(bipath).Should().Be.True();
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(bipath, bicollectionMapper.Object);
			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));
		}

		[Test]
		public void WhenManyToManyBidirectionalOtherMasterThenApplyMasterOtherToEntityClassInBoth()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass) || t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			pattern.Match(path).Should().Be.True();
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(path, collectionMapper.Object);
			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			pattern.Match(bipath).Should().Be.True();
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();
			pattern.Apply(bipath, bicollectionMapper.Object);
			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "MyBidirectToMyClass")));
		}
	}
}