using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class SubclassCustomizerCallingTest
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
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => typeof (MyClass) == t))).Returns(true);
			orm.Setup(m => m.IsTablePerClassHierarchy(It.IsAny<Type>())).Returns(true);
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

			mapper.Subclass<Inherited>(ca =>
				{
					ca.Should().Not.Be.Null();
					isCalled = true;
				});

			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			isCalled.Should().Be(true);
		}

		[Test]
		public void WhenCustomizeDiscriminatorValueThenSetDiscriminatorValue()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			mapper.Subclass<Inherited>(ca => ca.DiscriminatorValue(123));

			var hbmEntity = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) }).SubClasses[0];

			hbmEntity.DiscriminatorValue.Should().Be("123");
		}

		[Test]
		public void CallCustomizerOfAttributes()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			mapper.Subclass<Inherited>(x =>
			{
				x.BatchSize(10);
				x.Lazy(false);
				x.EntityName("myName");
				x.DynamicInsert(true);
				x.DynamicUpdate(true);
				x.SelectBeforeUpdate(true);
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.SubClasses.Single();
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
			mapper.Subclass<Inherited>(x =>
			{
				x.Loader("organization");
				x.SqlInsert("INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
				x.SqlUpdate("UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
				x.SqlDelete("DELETE FROM ORGANIZATION WHERE ORGID=?");
			});

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });
			var hbmClass = mappings.SubClasses.Single();
			hbmClass.SqlLoader.Satisfy(l => l != null && l.queryref == "organization");
			hbmClass.SqlInsert.Satisfy(l => l != null && l.Text[0] == "INSERT INTO ORGANIZATION (NAME, ORGID) VALUES ( UPPER(?), ? )");
			hbmClass.SqlUpdate.Satisfy(l => l != null && l.Text[0] == "UPDATE ORGANIZATION SET NAME=UPPER(?) WHERE ORGID=?");
			hbmClass.SqlDelete.Satisfy(l => l != null && l.Text[0] == "DELETE FROM ORGANIZATION WHERE ORGID=?");
		}
	}
}