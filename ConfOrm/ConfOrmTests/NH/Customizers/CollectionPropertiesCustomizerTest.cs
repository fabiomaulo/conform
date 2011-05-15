using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.Customizers
{
	public class CollectionPropertiesCustomizerTest
	{
		private class MyClass
		{
			public IEnumerable<MyEle> MyCollection { get; set; }
		}
		private class MyEle
		{
			public string Name { get; set; }
		}

		[Test]
		public void InvokeDirectMethods()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionPropertiesCustomizer<MyClass, MyEle>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();

			customizer.Inverse(true);
			customizer.Mutable(true);
			customizer.Where("aa");
			customizer.BatchSize(10);
			customizer.Lazy(CollectionLazy.Extra);
			customizer.OrderBy(x=>x.Name);
			customizer.Sort();
			customizer.Sort<object>();
			customizer.Cascade(CascadeOn.DeleteOrphans);
			customizer.Type<FakeUserCollectionType>();
			customizer.Type(typeof(FakeUserCollectionType));
			customizer.Table("table");
			customizer.Catalog("catalog");
			customizer.Schema("schema");
			customizer.OptimisticLock(true);
			customizer.Access(Accessor.NoSetter);
			customizer.Access(typeof(object)); // <== only to check the call

			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			collectionMapper.Verify(x => x.Inverse(It.Is<bool>(v => v)), Times.Once());
			collectionMapper.Verify(x => x.Mutable(It.Is<bool>(v => v)), Times.Once());
			collectionMapper.Verify(x => x.Where(It.Is<string>(v => v == "aa")), Times.Once());
			collectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
			collectionMapper.Verify(x => x.Lazy(It.Is<CollectionLazy>(v => v == CollectionLazy.Extra)), Times.Once());
			collectionMapper.Verify(x => x.OrderBy(It.Is<MemberInfo>(v => v == ForClass<MyEle>.Property(p=>p.Name))), Times.Once());
			collectionMapper.Verify(x => x.Sort(), Times.Once());
			collectionMapper.Verify(x => x.Sort<object>(), Times.Once());
			collectionMapper.Verify(x => x.Cascade(It.Is<CascadeOn>(v => v == CascadeOn.DeleteOrphans)), Times.Once());
			collectionMapper.Verify(x => x.Type<FakeUserCollectionType>(), Times.Once());
			collectionMapper.Verify(x => x.Type(It.Is<Type>(v => v == typeof(FakeUserCollectionType))), Times.Once());
			collectionMapper.Verify(x => x.Table(It.Is<string>(v => v == "table")), Times.Once());
			collectionMapper.Verify(x => x.Catalog(It.Is<string>(v => v == "catalog")), Times.Once());
			collectionMapper.Verify(x => x.Schema(It.Is<string>(v => v == "schema")), Times.Once());
			collectionMapper.Verify(x => x.OptimisticLock(It.Is<bool>(v => v)), Times.Once());
			collectionMapper.Verify(x => x.Access(It.Is<Accessor>(v => v == Accessor.NoSetter)), Times.Once());
			collectionMapper.Verify(x => x.Access(It.IsAny<Type>()), Times.Once());
		}

		[Test]
		public void InvokeCache()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionPropertiesCustomizer<MyClass, MyEle>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();
			var cacheMapper = new Mock<ICacheMapper>();
			collectionMapper.Setup(x => x.Cache(It.IsAny<Action<ICacheMapper>>())).Callback<Action<ICacheMapper>>(
				x => x.Invoke(cacheMapper.Object));

			customizer.Cache(x=> x.Region("static"));
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			cacheMapper.Verify(x => x.Region(It.Is<string>(v => v == "static")));
		}

		[Test]
		public void InvokeFilter()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionPropertiesCustomizer<MyClass, MyEle>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();
			var filterMapper = new Mock<IFilterMapper>();
			collectionMapper.Setup(x => x.Filter(It.IsAny<string>(), It.IsAny<Action<IFilterMapper>>())).Callback<string, Action<IFilterMapper>>(
				(fn, x) => x.Invoke(filterMapper.Object));

			customizer.Filter("myfilter", x => x.Condition("condition"));
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			filterMapper.Verify(x => x.Condition(It.Is<string>(v => v == "condition")));
		}
	}
}