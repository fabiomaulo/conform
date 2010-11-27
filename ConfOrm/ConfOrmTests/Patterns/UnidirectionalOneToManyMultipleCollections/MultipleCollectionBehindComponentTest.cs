using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.UnidirectionalOneToManyMultipleCollections
{
	public class MultipleCollectionBehindComponentTest
	{
		private class MyClass
		{
			public string Something { get; set; }
			public IEnumerable<Related> Relateds { get; set; }
			public IEnumerable<Bidirectional> Children { get; set; }
			public IEnumerable<Component> Components { get; set; }
			public IEnumerable<string> Elements { get; set; }
			public IDictionary<string, Related> DicRelateds { get; set; }
			public IDictionary<string, Bidirectional> DicChildren { get; set; }
		}

		private class Related
		{

		}

		private class Bidirectional
		{
			public MyClass MyClass { get; set; }
		}

		private class Component
		{
		}

		public class Contact
		{
			public int Id { get; set; }
			public IEnumerable<JobRecord> CurrentPositions { get; set; }
			public MyComponent Component { get; set; }
		}

		public class MyComponent
		{
			public IEnumerable<JobRecord> PastPositions { get; set; }			
		}

		public class JobRecord
		{
			public int Id { get; set; }
			public string Position { get; set; }
		}
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNoPropertyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			pattern.Match(null).Should().Be.False();
		}

		[Test]
		public void WhenNoCollectionPropertyThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Something));
			pattern.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfComponentsThenNoMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Components));
			pattern.Match(property).Should().Be.False();
		}

		private Mock<IDomainInspector> GetDomainInspectorMock()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] { typeof(MyClass), typeof(Related), typeof(Bidirectional) }).Contains(t)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(It.Is<Type>(t => t == typeof(Component)))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(MyClass), It.Is<Type>(t => (new[] { typeof(Related), typeof(Bidirectional) }).Contains(t)))).Returns(true);
			return orm;
		}

		private Mock<IDomainInspector> GetDomainInspectorMockWithBidiPropExclusion()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.Is<MemberInfo>(m => !m.Equals(ForClass<Bidirectional>.Property(y => y.MyClass))))).Returns(true);
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] { typeof(MyClass), typeof(Related), typeof(Bidirectional) }).Contains(t)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(It.Is<Type>(t => t == typeof(Component)))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(MyClass), It.Is<Type>(t => (new[] { typeof(Related), typeof(Bidirectional) }).Contains(t)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenCollectionBidirectionalThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Children));
			pattern.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenCollectionSeemsBidirectionalWithPropExclusionThenMatch()
		{
			var orm = GetDomainInspectorMockWithBidiPropExclusion();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Children));
			pattern.Match(property).Should().Be.True();
		}

		[Test]
		public void WhenCollectionOfElementsThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Elements));
			pattern.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenCollectionUnidirectionalThenMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Relateds));
			pattern.Match(property).Should().Be.True();
		}

		[Test]
		public void WhenDictionaryBidirectionalThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.DicChildren));
			pattern.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenDictionarySeemsBidirectionalWithPropExclusionThenMatch()
		{
			var orm = GetDomainInspectorMockWithBidiPropExclusion();
			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.DicChildren));
			pattern.Match(property).Should().Be.True();
		}

		[Test]
		public void WhenDictionaryUnidirectionalThenMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.DicRelateds));
			pattern.Match(property).Should().Be.True();
		}

		private Mock<IDomainInspector> GetDomainInspectorMockForBaseTests()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] { typeof(Contact), typeof(JobRecord) }).Contains(t)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(typeof (MyComponent))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(Contact), typeof(JobRecord))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(MyComponent), typeof(JobRecord))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenMultipleUsageBehindAComponentThenMatch()
		{
			var orm = GetDomainInspectorMockForBaseTests();

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var componentProperty = new PropertyPath(null, ForClass<Contact>.Property(x => x.Component));
			var property = new PropertyPath(componentProperty, ForClass<MyComponent>.Property(x => x.PastPositions));
			applier.Match(property).Should().Be.True();
		}

		[Test]
		public void ApplyPropertyPathAndPostFixForPastPositions()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			var componentProperty = new PropertyPath(null, ForClass<Contact>.Property(x => x.Component));
			var property = new PropertyPath(componentProperty, ForClass<MyComponent>.Property(x => x.PastPositions));
			applier.Apply(property, collectionMapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "ContactComponentPastPositions_key")));
		}
	}
}