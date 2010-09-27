using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.CoolNamingsAppliersTests
{
	public class CollectionOfComponentsPropertyPathKeyColumnApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyComponent Component { get; set; }
			public ICollection<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyComponent> Components { get; set; }
		}

		[Test]
		public void WhenCollectionIsInPlainEntityThenApplyClassNamePropertyPathId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPropertyPathKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Components));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassComponentsId")));
		}

		[Test]
		public void WhenCollectionIsInsideComponentThenApplyClassNamePropertyPathId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsPropertyPathKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Components));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassComponentComponentsId")));
		}
	}
}