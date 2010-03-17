using System;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class UnionSubclassAppliersCallingTest
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
			orm.Setup(m => m.IsTablePerConcreteClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void ApplierCalledPerSubclass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			var applier = new Mock<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			applier.Setup(x => x.Match(It.IsAny<Type>())).Returns(true);

			mapper.PatternsAppliers.UnionSubclass.Add(applier.Object);
			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			applier.Verify(x => x.Match(It.Is<Type>(t => t == typeof(Inherited))), Times.Once());
			applier.Verify(x => x.Apply(It.Is<Type>(t => t == typeof(Inherited)), It.Is<IUnionSubclassAttributesMapper>(cm => cm != null)), Times.Once());
		}
	}
}