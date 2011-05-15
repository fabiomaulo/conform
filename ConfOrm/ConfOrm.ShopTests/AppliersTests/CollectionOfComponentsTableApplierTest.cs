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
	public class CollectionOfComponentsTableApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<MyComponent> Components { get; set; }
			public ICollection<string> Elements { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<MyComponent> Components { get; set; }
			public ICollection<string> Elements { get; set; }
		}

		private class MyRelated
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Something));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsManyToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(new PropertyPath(null, ForClass<MyClass>.Property(p => p.Components)), ForClass<MyComponent>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsElementsThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Elements));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsElementsInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			var path = new PropertyPath(new PropertyPath(null, ForClass<MyClass>.Property(p => p.Components)), ForClass<MyComponent>.Property(p => p.Elements));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenComponentPropertyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenComponentsCollectionThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Components));
			pattern.Match(path).Should().Be.True();
		}

		[Test]
		public void WhenComponentsCollectionInsideComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);
			var path = new PropertyPath(new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component)), ForClass<MyComponent>.Property(p => p.Components));

			pattern.Match(path).Should().Be.True();
		}

		[Test]
		public void WhenCollectionIsInPlainEntityThenApplyClassNamePropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Components));

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, mapper.Object);
			mapper.Verify(km => km.Table(It.Is<string>(s => s == "MyClassComponents")));
		}

		[Test]
		public void WhenCollectionIsInsideComponentThenApplyClassNameComponentPropertyNamePropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfComponentsTableApplier(orm.Object);
			orm.Setup(x => x.IsComponent(typeof(MyComponent))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Components));

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, mapper.Object);
			mapper.Verify(km => km.Table(It.Is<string>(s => s == "MyClassComponentComponents")));
		}

	}
}