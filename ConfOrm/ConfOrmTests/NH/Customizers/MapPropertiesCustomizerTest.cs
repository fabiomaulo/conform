using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class MapPropertiesCustomizerTest
	{
		private class MyClass
		{
			public IDictionary<MyKey, int> MyCollection { get; set; }
		}

		private class MyKey
		{
			
		}

		[Test]
		public void InvokeMapKeyManyToMany()
		{
			var propertyPath = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new MapPropertiesCustomizer<MyClass, MyKey, int>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<IMapPropertiesMapper>();
			var mapIndexMapper = new Mock<IMapKeyManyToManyMapper>();
			collectionMapper.Setup(x => x.MapKeyManyToMany(It.IsAny<Action<IMapKeyManyToManyMapper>>())).Callback<Action<IMapKeyManyToManyMapper>>(
				x => x.Invoke(mapIndexMapper.Object));

			customizer.MapKeyManyToMany(x => x.ForeignKey("FKMia"));
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			mapIndexMapper.Verify(x => x.ForeignKey(It.Is<string>(v => v == "FKMia")));
		}
	}
}