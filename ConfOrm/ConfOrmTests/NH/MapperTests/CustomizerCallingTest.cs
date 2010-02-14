using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class CustomizerCallingTest
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
			bool simplePropertyCalled = false;
			bool bagCalled = false;
			bool listCalled = false;
			bool setCalled = false;
			bool mapCalled = false;
			bool manyToOneCalled = false;
			bool oneToOneCalled = false;

			mapper.Customize<MyClass>(x =>
				{
					x.Property(mc => mc.SimpleProperty, pm => simplePropertyCalled = true);
					x.Collection(mc => mc.Bag, pm => bagCalled = true);
					x.Collection(mc => mc.List, pm => listCalled = true);
					x.Collection(mc => mc.Set, pm => setCalled = true);
					x.Collection(mc => mc.Map, pm => mapCalled = true);
					x.ManyToOne(mc => mc.ManyToOne, pm => manyToOneCalled = true);
					x.OneToOne(mc => mc.OneToOne, pm => oneToOneCalled = true);
				});

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			simplePropertyCalled.Should().Be.True();
			bagCalled.Should().Be.True();
			listCalled.Should().Be.True();
			setCalled.Should().Be.True();
			mapCalled.Should().Be.True();
			manyToOneCalled.Should().Be.True();
			oneToOneCalled.Should().Be.True();
		}
	}
}