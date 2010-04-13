using System;
using System.Collections;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class CollectionOfElementsKeyColumnApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public IDictionary<string, MyRelated> MapRelationOnValue { get; set; }
			public IDictionary<MyRelated, string > MapRelationOnKey { get; set; }
			public IDictionary<string, string> MapOfStrings { get; set; }
			public ICollection<string> Strings { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyRelated> Relateds { get; set; }
			public ICollection<string> Strings { get; set; }
		}

		private class MyRelated
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Something));
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsManyToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapRelationOnValue));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapKeyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapRelationOnKey));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenNoRelationBetweenEntitiesThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			var pathCollection = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Strings));
			var pathMap = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapOfStrings));
			
			pattern.Match(pathCollection).Should().Be.True();
			pattern.Match(pathMap).Should().Be.True();
		}

		[Test]
		public void WhenNoRelationWithComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);
			var pathCollection = new PropertyPath(null, ForClass<MyComponent>.Property(p => p.Strings));

			pattern.Match(pathCollection).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsInPlainEntityThenApplyClassNameId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Strings));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassId")));
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenApplyClassNameComponentPropertyNameId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Strings));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassId")));
		}
	}
}