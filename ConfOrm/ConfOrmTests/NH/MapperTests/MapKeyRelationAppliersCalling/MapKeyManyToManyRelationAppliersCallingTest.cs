using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests.MapKeyRelationAppliersCalling
{
	public class MapKeyManyToManyRelationAppliersCallingTest
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
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(p => p.Dictionary)))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(Relation)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenRegisterApplierOnMapKeyManyToManyThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var applier = new Mock<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			mapper.PatternsAppliers.MapKeyManyToMany.Add(applier.Object);

			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(p => p.Dictionary))));
			applier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(p => p.Dictionary)), It.Is<IMapKeyManyToManyMapper>(mkm => mkm != null)));
		}

		[Test]
		public void WhenRegisterApplierOnMapKeyManyToManyPathThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var applier = new Mock<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			mapper.PatternsAppliers.MapKeyManyToManyPath.Add(applier.Object);

			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<PropertyPath>(pp => pp.LocalMember == ForClass<MyClass>.Property(p => p.Dictionary))));
			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember == ForClass<MyClass>.Property(p => p.Dictionary)), It.Is<IMapKeyManyToManyMapper>(mkm => mkm != null)));
		}

	}
}