using System;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
namespace ConfOrmTests.NH.MapperTests
{
	public class RootClassAppliersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
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
		public void ApplierCalledPerRootClass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			var applier = new Mock<IPatternApplier<Type, IClassAttributesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<Type>())).Returns(true);

			mapper.PatternsAppliers.RootClass.Add(applier.Object);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<Type>(t => t == typeof(MyClass))), Times.Once());
			applier.Verify(x => x.Apply(It.Is<Type>(t => t == typeof(MyClass)), It.Is<IClassAttributesMapper>(cm => cm != null)), Times.Once());
		}
	}
}