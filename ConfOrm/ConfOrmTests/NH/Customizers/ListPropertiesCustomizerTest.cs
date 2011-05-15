using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class ListPropertiesCustomizerTest
	{
		private class MyClass
		{
			public IEnumerable<int> MyCollection { get; set; }
		}

		[Test]
		public void InvokeIndex()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new ListPropertiesCustomizer<MyClass, int>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<IListPropertiesMapper>();
			var listIndexMapper = new Mock<IListIndexMapper>();
			collectionMapper.Setup(x => x.Index(It.IsAny<Action<IListIndexMapper>>())).Callback<Action<IListIndexMapper>>(
				x => x.Invoke(listIndexMapper.Object));

			customizer.Index(x => x.Base(1));
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			listIndexMapper.Verify(x => x.Base(It.Is<int>(v => v == 1)));
		}
	}
}