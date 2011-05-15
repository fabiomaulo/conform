using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.CoolNamingsAppliersTests
{
	public class CollectionOfElementsColumnApplierTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyRelated> Relateds { get; set; }
			public MyComponent Component { get; set; }
			public IList Something { get; set; }
			public IDictionary<string, MyRelated> MapRelationOnValue { get; set; }
			public IDictionary<MyRelated, string> MapRelationOnKey { get; set; }
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
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Something));
			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsManyToManyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyInsideComponentThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyComponent)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);

			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Relateds));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapValueThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			orm.Setup(x => x.IsOneToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapRelationOnValue));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenRelationIsOneToManyForMapKeyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyRelated)))).Returns(true);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapRelationOnKey));

			pattern.Match(path).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfElementsInsideEntityThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			var pathCollection = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Strings));
			var pathMap = new PropertyPath(null, ForClass<MyClass>.Property(p => p.MapOfStrings));

			pattern.Match(pathCollection).Should().Be.True();
			pattern.Match(pathMap).Should().Be.True();
		}

		[Test]
		public void WhenCollectionOfElementsInsideComponentThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);
			var pathCollection = new PropertyPath(null, ForClass<MyComponent>.Property(p => p.Strings));

			pattern.Match(pathCollection).Should().Be.True();
		}

		[Test]
		public void WhenCollectionOfElementsInsideEntityThenApplyPropertyNameElement()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);

			var mapper = new Mock<IElementMapper>();
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Strings));

			pattern.Apply(path, mapper.Object);
			mapper.Verify(elementMapper => elementMapper.Column(It.Is<string>(s => s == "StringsElement")));
		}

		[Test]
		public void WhenCollectionOfElementsInsideComponentThenApplyPropertyPathNameElement()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new CollectionOfElementsColumnApplier(orm.Object);

			var mapper = new Mock<IElementMapper>();
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(p => p.Strings));

			pattern.Apply(path, mapper.Object);
			mapper.Verify(elementMapper => elementMapper.Column(It.Is<string>(s => s == "ComponentStringsElement")));
		}
	}
}