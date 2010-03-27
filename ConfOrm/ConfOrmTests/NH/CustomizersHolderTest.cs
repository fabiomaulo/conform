using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Mappers;
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
		}

		[Test]
		public void InvokingCustomizerOnSetThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<ISetPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnBagThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IBagPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnListThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IListPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}

		[Test]
		public void InvokingCustomizerOnMapThenInvokeCollectionPropertiesCustomizer()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var concreteCollectionMapper = new Mock<IMapPropertiesMapper>();

			customizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(10));
			customizersHolder.InvokeCustomizers(propertyPath, concreteCollectionMapper.Object);

			concreteCollectionMapper.Verify(x => x.BatchSize(It.Is<int>(v => v == 10)), Times.Once());
		}
	}
}