using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests.MapKeyRelationAppliersCalling
{
	public class MapKeyRelationAppliersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public IDictionary<string ,string> Dictionary { get; set; }
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
			return orm;
		}

		[Test]
		public void WhenRegisterApplierOnMapKeyThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var applier = new Mock<IPatternApplier<MemberInfo, IMapKeyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);
			mapper.PatternsAppliers.MapKey.Add(applier.Object);

			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<MemberInfo>(mi => mi == ConfOrm.ForClass<MyClass>.Property(p => p.Dictionary))));
			applier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi == ConfOrm.ForClass<MyClass>.Property(p => p.Dictionary)), It.Is<IMapKeyMapper>(mkm => mkm != null)));
		}

		[Test]
		public void WhenRegisterApplierOnMapKeyPathThenInvokeApplier()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var applier = new Mock<IPatternApplier<PropertyPath, IMapKeyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<PropertyPath>())).Returns(true);
			mapper.PatternsAppliers.MapKeyPath.Add(applier.Object);

			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<PropertyPath>(pp => pp.LocalMember == ConfOrm.ForClass<MyClass>.Property(p => p.Dictionary))));
			applier.Verify(x => x.Apply(It.Is<PropertyPath>(pp => pp.LocalMember == ConfOrm.ForClass<MyClass>.Property(p => p.Dictionary)), It.Is<IMapKeyMapper>(mkm => mkm != null)));
		}
	}
}