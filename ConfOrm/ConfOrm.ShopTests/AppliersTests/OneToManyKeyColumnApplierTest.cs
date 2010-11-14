using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class OneToManyKeyColumnApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public IDictionary<string, MyRelated> Map { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyRelated> Relateds { get; set; }
		}

		private class MyRelated
		{
			public int Id { get; set; }
		}

		private class Parent
		{
			public int Id { get; set; }
			public ICollection<Child> Children	{ get; set; }
			public ComponentWithChild Component { get; set; }
		}

		private class Child
		{
			public Parent MyParent { get; set; }
		}

		private class ComponentWithChild
		{
			public ICollection<Child> Children { get; set; }			
		}

		[Test]
		public void WhenNoGenericCollectionThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Something));
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Map));

			pattern.Match(path).Should().Be.True();
		}

		[Test]
		public void WhenRelationIsNotOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(false);
			var pathCollection = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));
			var pathMap = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Map));

			pattern.Match(pathCollection).Should().Be.False();
			pattern.Match(pathMap).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenApplyClassNameId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassId")));
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenApplyClassNameComponentPropertyNameId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Relateds));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassComponentId")));
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenApplyClassNameId()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Map));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyClassId")));
		}

		[Test]
		public void WhenParentChildThenApplyPropertyNameIdOfChild()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<Parent>.Property(p => p.Children));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyParentId")));
		}

		[Test]
		public void WhenParentChildInComponentThenApplyPropertyNameIdOfChild()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(ComponentWithChild)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var level0 = new PropertyPath(null, ForClass<Parent>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<ComponentWithChild>.Property(p => p.Children));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "MyParentId")));
		}
	}
}