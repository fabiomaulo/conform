using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class JoinedSubclassCustomizerCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		private class Inherited : MyClass
		{
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => typeof(MyClass) == t))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void CustomizerCalledPerSubclass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			bool isCalled = false;

			mapper.JoinedSubclass<Inherited>(ca =>
			{
				ca.Should().Not.Be.Null();
				isCalled = true;
			});

			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			isCalled.Should().Be(true);
		}

		[Test]
		public void InvokeCustomizers()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			Mock<ICustomizersHolder> customizers = new Mock<ICustomizersHolder>();
			var mapper = new Mapper(orm.Object, customizers.Object);
			mapper.JoinedSubclass<Inherited>(ca =>{ });

			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			customizers.Verify(
				c => c.InvokeCustomizers(It.Is<Type>(t => t == typeof (Inherited)), It.IsAny<IJoinedSubclassMapper>()));
		}

		[Test]
		public void SetTableSpecifications()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var mapper = new Mapper(orm.Object);
			mapper.JoinedSubclass<Inherited>(x =>
			{
				x.Table("tabella");
				x.Schema("dbo");
				x.Catalog("catalogo");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.JoinedSubclasses.Single();
			hbmClass.table.Should().Be("tabella");
			hbmClass.catalog.Should().Be("catalogo");
			hbmClass.schema.Should().Be("dbo");
		}


		[Test]
		public void CallCustomizerOfAttributes()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			mapper.JoinedSubclass<Inherited>(x =>
			{
				x.BatchSize(10);
				x.Lazy(false);
				x.EntityName("myName");
				x.DynamicInsert(true);
				x.DynamicUpdate(true);
				x.SelectBeforeUpdate(true);
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.JoinedSubclasses.Single();
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
			mapper.JoinedSubclass<Inherited>(x =>
			{
				x.Loader("organization");
				x.SqlInsert("INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
				x.SqlUpdate("UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
				x.SqlDelete("DELETE FROM ORGANIZATION WHERE ORGID=?");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.JoinedSubclasses.Single();
			hbmClass.SqlLoader.Satisfy(l => l != null && l.queryref == "organization");
			hbmClass.SqlInsert.Satisfy(l => l != null && l.Text[0] == "INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
			hbmClass.SqlUpdate.Satisfy(l => l != null && l.Text[0] == "UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
			hbmClass.SqlDelete.Satisfy(l => l != null && l.Text[0] == "DELETE FROM ORGANIZATION WHERE ORGID=?");
		}

		[Test]
		public void CallCustomizerOfKey()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			mapper.JoinedSubclass<Inherited>(x => x.Key(km=> km.Column("pizzaId")));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.JoinedSubclasses.Single();
			hbmClass.key.column1.Should().Be("pizzaId");
		}
	}
}