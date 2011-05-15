using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class PatternsAppliersOnCollectionCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string SimpleProperty { get; set; }
			public ICollection<string> Bag { get; set; }
			public IList<string> List { get; set; }
			public ISet<string> Set { get; set; }
			public IDictionary<string, MyOneToManyRelated> Map { get; set; }
			public MyManyToOneRelated ManyToOne { get; set; }
			public MyOneToOneRelated OneToOne { get; set; }
			public object Any { get; set; }
			public ISet<MyManyToManyRelated> SetOfMany { get; set; }
		}
		private class MyManyToOneRelated
		{
			public int Id { get; set; }
		}
		private class MyOneToOneRelated
		{
			public int Id { get; set; }
		}
		private class MyOneToManyRelated
		{
			public int Id { get; set; }
		}
		private class MyManyToManyRelated
		{
			public int Id { get; set; }
		}

		Mock<IDomainInspector> domainInspectorMock;

		[TestFixtureSetUp]
		public void CreateDomainInspectorMock()
		{
			domainInspectorMock = new Mock<IDomainInspector>();
			domainInspectorMock.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			domainInspectorMock.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			domainInspectorMock.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			domainInspectorMock.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			domainInspectorMock.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			domainInspectorMock.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Bag")))).Returns(true);
			domainInspectorMock.Setup(m => m.IsList(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("List")))).Returns(true);
			domainInspectorMock.Setup(m => m.IsSet(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Set")))).Returns(true);
			domainInspectorMock.Setup(m => m.IsSet(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("SetOfMany")))).Returns(true);
			domainInspectorMock.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Map")))).Returns(true);
			domainInspectorMock.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyManyToOneRelated)))).Returns(true);
			domainInspectorMock.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyOneToOneRelated)))).Returns(true);
			domainInspectorMock.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyManyToManyRelated)))).Returns(true);
			domainInspectorMock.Setup(m => m.IsHeterogeneousAssociation(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Any")))).Returns(true);
		}

		[Test]
		public void CallAppliersOnSimpleProperty()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();
			
			var applier = new Mock<IPatternApplier<PropertyPath, IPropertyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.PropertyPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.IsAny<PropertyPath>(), It.IsAny<IPropertyMapper>()));
		}

		[Test]
		public void CallAppliersOnBag()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.CollectionPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp=> pp.LocalMember.Name == "Bag"), It.IsAny<ICollectionPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnBagPath()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IBagPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.BagPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "Bag"), It.IsAny<IBagPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnBag()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<MemberInfo, IBagPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			patternAppliers.Bag.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<MemberInfo>(pp => pp.Name == "Bag"), It.IsAny<IBagPropertiesMapper>()));
		}

		[Test]
		public void CallAppliersOnList()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.CollectionPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "List"), It.IsAny<ICollectionPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnListPath()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IListPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.ListPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "List"), It.IsAny<IListPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnList()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<MemberInfo, IListPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			patternAppliers.List.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<MemberInfo>(pp => pp.Name == "List"), It.IsAny<IListPropertiesMapper>()));
		}

		[Test]
		public void CallAppliersOnSet()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.CollectionPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "Set"), It.IsAny<ICollectionPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnSetPath()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, ISetPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.SetPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "Set"), It.IsAny<ISetPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnSet()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<MemberInfo, ISetPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			patternAppliers.Set.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<MemberInfo>(pp => pp.Name == "Set"), It.IsAny<ISetPropertiesMapper>()));
		}

		[Test]
		public void CallAppliersOnMap()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.CollectionPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "Map"), It.IsAny<ICollectionPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnMapPath()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IMapPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.MapPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember.Name == "Map"), It.IsAny<IMapPropertiesMapper>()));
		}

		[Test]
		public void CallSpecificAppliersOnMap()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<MemberInfo, IMapPropertiesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			patternAppliers.Map.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.Is<MemberInfo>(pp => pp.Name == "Map"), It.IsAny<IMapPropertiesMapper>()));
		}

		[Test]
		public void CallAppliersOnManyToOne()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IManyToOneMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.ManyToOnePath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.IsAny<PropertyPath>(), It.IsAny<IManyToOneMapper>()));
		}

		[Test]
		public void CallAppliersOnOneToOne()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IOneToOneMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.OneToOnePath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.IsAny<PropertyPath>(), It.IsAny<IOneToOneMapper>()));
		}

		[Test]
		public void CallAppliersOnAny()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IAnyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.AnyPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.IsAny<PropertyPath>(), It.IsAny<IAnyMapper>()));
		}

		[Test]
		public void CallAppliersOnManyToMany()
		{
			var patternAppliers = new EmptyPatternsAppliersHolder();

			var applier = new Mock<IPatternApplier<PropertyPath, IManyToManyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			patternAppliers.ManyToManyPath.Add(applier.Object);

			var mapper = new Mapper(domainInspectorMock.Object, patternAppliers);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Apply(It.IsAny<PropertyPath>(), It.IsAny<IManyToManyMapper>()));
		}
	}
}