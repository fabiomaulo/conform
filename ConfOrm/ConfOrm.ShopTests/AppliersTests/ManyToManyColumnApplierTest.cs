using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToManyColumnApplierTest
	{
		private class MyClass
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
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
		public void WhenManyToManyCollectionThenApplyColumnNameByRelatedEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var mayToManyMapper = new Mock<IManyToManyMapper>();

			pattern.Apply(path, mayToManyMapper.Object);

			mayToManyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyBidirectId")));
		}

		[Test]
		public void WhenManyToManyCollectionInsideComponentThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyColumnApplier(orm.Object);

			var pathEntity = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyComponent));
			var path = new PropertyPath(pathEntity, ForClass<MyComponent>.Property(x => x.MyBidirects));
			var mayToManyMapper = new Mock<IManyToManyMapper>();

			pattern.Apply(path, mayToManyMapper.Object);

			mayToManyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyBidirectId")));
		}

		[Test]
		public void WhenManyToManyDictionaryThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);

			var pattern = new ManyToManyColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MapValue));
			var mayToManyMapper = new Mock<IManyToManyMapper>();

			pattern.Apply(path, mayToManyMapper.Object);

			mayToManyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyBidirectId")));
		}

		[Test]
		public void WhenManyToManyBidirectionalThenApplyColumnNameByEntityClass()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);

			var pattern = new ManyToManyColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var mayToManyMapper = new Mock<IManyToManyMapper>();

			pattern.Apply(path, mayToManyMapper.Object);

			mayToManyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyBidirectId")));

			var bipath = new PropertyPath(null, ForClass<MyBidirect>.Property(x => x.MyClasses));
			var bimayToManyMapper = new Mock<IManyToManyMapper>();

			pattern.Apply(bipath, bimayToManyMapper.Object);

			bimayToManyMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyClassId")));
		}
	}
}