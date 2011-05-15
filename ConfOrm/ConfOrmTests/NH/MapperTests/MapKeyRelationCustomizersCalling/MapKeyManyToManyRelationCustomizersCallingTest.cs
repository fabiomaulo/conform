using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests.MapKeyRelationCustomizersCalling
{
	public class MapKeyManyToManyRelationCustomizersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public IDictionary<Relation, string> Dictionary { get; set; }
		}

		private class Relation
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
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ConfOrm.ForClass<MyClass>.Property(p => p.Dictionary)))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(Relation)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenRegisterCustomizerOnMapKeyManyToManyThenInvokeCustomizer()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var called = false;
			mapper.Class<MyClass>(cm=> cm.Map(myClass=> myClass.Dictionary,mpm=> { },mkm=> called= true, cerm=> { }));
			mapper.CompileMappingFor(new[] { typeof(MyClass) });
			called.Should().Be.True();
		}

		[Test]
		public void WhenRegisterCustomizerOnMapKeyManyToManyThenInvokeCustomizerActions()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			mapper.Class<MyClass>(cm => cm.Map(myClass => myClass.Dictionary, mpm => { }, mkm => mkm.ManyToMany(mtm => mtm.Column("RelationId")), cerm => { }));
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var rc = mapping.RootClasses.Single();
			var hbmMap = rc.Properties.OfType<HbmMap>().Single();
			var hbmMapKeyManyToMany = (HbmMapKeyManyToMany) hbmMap.Item;
			hbmMapKeyManyToMany.column.Should().Be("RelationId");
		}
	}
}