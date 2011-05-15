using System;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class VersionAppliersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int Version { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => typeof(MyClass) == t))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsVersion(It.Is<MemberInfo>(mi => mi.Name == "Version"))).Returns(true);
			return orm;
		}

		[Test]
		public void ApplierCalledPerVersion()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			var applier = new Mock<IPatternApplier<MemberInfo, IVersionMapper>>();
			applier.Setup(x => x.Match(It.Is<MemberInfo>(mi => mi.Name == "Version"))).Returns(true);

			mapper.PatternsAppliers.Version.Add(applier.Object);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<MemberInfo>(member => member == ConfOrm.ForClass<MyClass>.Property(c => c.Version))), Times.Once());
			applier.Verify(x => x.Apply(It.Is<MemberInfo>(member => member == ConfOrm.ForClass<MyClass>.Property(c => c.Version)), It.Is<IVersionMapper>(vm => vm != null)), Times.Once());
		}
	}
}