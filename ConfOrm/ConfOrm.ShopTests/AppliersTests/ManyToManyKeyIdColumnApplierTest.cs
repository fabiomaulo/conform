using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToManyKeyIdColumnApplierTest
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
		public void WhenManyToManyCollectionThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyKeyIdColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(x => x.Invoke(keyMapper.Object));

			pattern.Apply(path, collectionMapper.Object);

			keyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyClassId")));
		}

		[Test]
		public void WhenManyToManyCollectionInsideComponentThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyKeyIdColumnApplier(orm.Object);

			var pathEntity = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyComponent));
			var path = new PropertyPath(pathEntity, ForClass<MyComponent>.Property(x => x.MyBidirects));
			
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(x => x.Invoke(keyMapper.Object));

			pattern.Apply(path, collectionMapper.Object);

			keyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyClassId")));
		}

		[Test]
		public void WhenManyToManyDictionaryThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyKeyIdColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MapKey));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(x => x.Invoke(keyMapper.Object));

			pattern.Apply(path, collectionMapper.Object);

			keyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyClassId")));
		}

		[Test]
		public void WhenManyToManyBidirectionalThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyKeyIdColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(x => x.Invoke(keyMapper.Object));

			pattern.Apply(path, collectionMapper.Object);

			keyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyClassId")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();
			var bikeyMapper = new Mock<IKeyMapper>();
			bicollectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(x => x.Invoke(bikeyMapper.Object));

			pattern.Apply(bipath, bicollectionMapper.Object);

			bikeyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyBidirectId")));
		}
	}
}