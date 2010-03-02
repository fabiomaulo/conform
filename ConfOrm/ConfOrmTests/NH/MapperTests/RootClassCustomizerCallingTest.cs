using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class RootClassCustomizerCallingTest
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

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Bag")))).Returns(true);
			orm.Setup(m => m.IsList(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("List")))).Returns(true);
			orm.Setup(m => m.IsSet(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Set")))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("Map")))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyManyToOneRelated)))).Returns(true);
			orm.Setup(m => m.IsOneToOne(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyOneToOneRelated)))).Returns(true);
			return orm;
		}

		[Test]
		public void CallCustomizerOfEachProperty()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool idCalled = false;
			bool simplePropertyCalled = false;
			bool bagCalled = false;
			bool listCalled = false;
			bool setCalled = false;
			bool mapCalled = false;
			bool manyToOneCalled = false;
			bool oneToOneCalled = false;

			mapper.Class<MyClass>(x =>
			{
				x.Id(c => c.Id, i => idCalled = true);
				x.BatchSize(10);
				x.Lazy(false);
				x.EntityName("myName");
				x.DynamicInsert(true);
				x.DynamicUpdate(true);
				x.SelectBeforeUpdate(true);
				x.Property(mc => mc.SimpleProperty, pm => simplePropertyCalled = true);
				x.Bag(mc => mc.Bag, pm => bagCalled = true, pm => { });
				x.List(mc => mc.List, pm => listCalled = true, pm => { });
				x.Set(mc => mc.Set, pm => setCalled = true, pm => { });
				x.Map(mc => mc.Map, pm => mapCalled = true, pm => { });
				x.ManyToOne(mc => mc.ManyToOne, pm => manyToOneCalled = true);
				x.OneToOne(mc => mc.OneToOne, pm => oneToOneCalled = true);
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			idCalled.Should().Be.True();
			simplePropertyCalled.Should().Be.True();
			bagCalled.Should().Be.True();
			listCalled.Should().Be.True();
			setCalled.Should().Be.True();
			mapCalled.Should().Be.True();
			manyToOneCalled.Should().Be.True();
			oneToOneCalled.Should().Be.True();
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.BatchSize.Should().Be(10);
			hbmClass.EntityName.Should().Be("myName");
			hbmClass.UseLazy.Should().Be(false);
			hbmClass.DynamicInsert.Should().Be(true);
			hbmClass.DynamicUpdate.Should().Be(true);
			hbmClass.SelectBeforeUpdate.Should().Be(true);
		}

		[Test]
		public void SetCustomQueries()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var mapper = new Mapper(orm.Object);
			mapper.Class<MyClass>(x =>
			{
				x.Loader("organization");
				x.SqlInsert("INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
				x.SqlUpdate("UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
				x.SqlDelete("DELETE FROM ORGANIZATION WHERE ORGID=?");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.SqlLoader.Satisfy(l => l != null && l.queryref == "organization");
			hbmClass.SqlInsert.Satisfy(l => l != null && l.Text[0] == "INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
			hbmClass.SqlUpdate.Satisfy(l => l != null && l.Text[0] == "UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
			hbmClass.SqlDelete.Satisfy(l => l != null && l.Text[0] == "DELETE FROM ORGANIZATION WHERE ORGID=?");
		}

		[Test]
		public void SetDiscriminatorValue()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClassHierarchy(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			mapper.Class<MyClass>(x =>
			{
				x.DiscriminatorValue(123);
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.discriminatorvalue.Should().Be("123");
		}

		[Test]
		public void SetTableSpecifications()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClassHierarchy(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			mapper.Class<MyClass>(x =>
			{
				x.Table("tabella");
				x.Schema("dbo");
				x.Catalog("catalogo");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = mappings.RootClasses.Single();
			hbmClass.table.Should().Be("tabella");
			hbmClass.catalog.Should().Be("catalogo");
			hbmClass.schema.Should().Be("dbo");
		}

		[Test]
		public void CallCustomizerOfIdWithoutProperty()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool idCalled = false;

			mapper.Class<MyClass>(x => x.Id(i => idCalled = true));

			mapper.CompileMappingFor(new[] { typeof(MyClass) });
			idCalled.Should().Be.True();
		}
	}
}