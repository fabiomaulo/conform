using System.Collections.Generic;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH
{
	public class CustomizersHolderTest
	{
		private class MyClass
		{
			public IEnumerable<int> MyCollection { get; set; }
			public IDictionary<string, int> MyDictionary { get; set; }
		}

		[Test]
		public void InvokingCustomizerOnSetThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<ISetPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnBagThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IBagPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnListThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IListPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnMapThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IMapPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokeCustomizerOfCollectionElementRelation()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var elementMapper = new Mock<IElementMapper>();

			customizersHolder.AddCustomizer(propertyPath, (IElementMapper x) => x.Length(10));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Length(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokeCustomizerOfCollectionOneToManyRelation()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var elementMapper = new Mock<IOneToManyMapper>();

			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.NotFound(NotFoundMode.Ignore));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.NotFound(It.Is<NotFoundMode>(v => v == NotFoundMode.Ignore)), Times.Once());
		}

		[Test]
		public void InvokeCustomizerOfCollectionManyToManyRelation()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var elementMapper = new Mock<IManyToManyMapper>();

			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Column("pizza"));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Column(It.Is<string>(v => v == "pizza")), Times.Once());
		}

		[Test]
		public void InvokeCustomizerOfDictionaryKeyManyToManyRelation()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyDictionary));
			var customizersHolder = new CustomizersHolder();
			var elementMapper = new Mock<IMapKeyManyToManyMapper>();

			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Column("pizza"));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Column(It.Is<string>(v => v == "pizza")), Times.Once());
		}

		[Test]
		public void InvokeCustomizerOfDictionaryKeyElementRelation()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyDictionary));
			var customizersHolder = new CustomizersHolder();
			var elementMapper = new Mock<IMapKeyMapper>();

			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Column("pizza"));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Column(It.Is<string>(v => v == "pizza")), Times.Once());
		}
	}
}