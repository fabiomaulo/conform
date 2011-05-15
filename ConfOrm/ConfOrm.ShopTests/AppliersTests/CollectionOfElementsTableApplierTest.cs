using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class CollectionOfElementsTableApplierTest
	{
		private class MyClass
		{
			public MyComponent Component { get; set; }
			public IDictionary<string, string> MapOfStrings { get; set; }
			public ICollection<string> Strings { get; set; }
		}

		private class MyComponent
		{
			public ICollection<string> Strings { get; set; }
		}

		[Test]
		public void WhenCollectionIsInPlainEntityThenApplyClassNamePropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsTableApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Strings));

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, mapper.Object);
			mapper.Verify(km => km.Table(It.Is<string>(s => s == "MyClassStrings")));
		}

		[Test]
		public void WhenDictionaryIsInPlainEntityThenApplyClassNamePropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsTableApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapOfStrings));

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, mapper.Object);
			mapper.Verify(km => km.Table(It.Is<string>(s => s == "MyClassMapOfStrings")));
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenApplyClassNameComponentPropertyNamePropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsTableApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Strings));

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, mapper.Object);
			mapper.Verify(km => km.Table(It.Is<string>(s => s == "MyClassComponentStrings")));
		}
	}
}