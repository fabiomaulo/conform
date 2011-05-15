using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class CollectionKeyCustomizerTest
	{
		private class MyClass
		{
			public int AProp { get; set; }
			public IEnumerable<int> MyCollection { get; set; }
		}

		[Test]
		public void InvokeColumn()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionKeyCustomizer<MyClass>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			customizer.Column("pizza");
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			keyMapper.Verify(x => x.Column(It.Is<string>(str => str == "pizza")), Times.Once());
		}

		[Test]
		public void InvokeOnDelete()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionKeyCustomizer<MyClass>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			customizer.OnDelete(OnDeleteAction.Cascade);
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			keyMapper.Verify(x => x.OnDelete(It.Is<OnDeleteAction>(v => v == OnDeleteAction.Cascade)), Times.Once());
		}

		[Test]
		public void InvokePropertyRef()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionKeyCustomizer<MyClass>(propertyPath, customizersHolder);
			var collectionMapper = new Mock<ISetPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			customizer.PropertyRef(x=> x.AProp);
			customizersHolder.InvokeCustomizers(propertyPath, collectionMapper.Object);

			keyMapper.Verify(x => x.PropertyRef(It.Is<MemberInfo>(v => v == ConfOrm.ForClass<MyClass>.Property(p => p.AProp))), Times.Once());
		}
	}
}