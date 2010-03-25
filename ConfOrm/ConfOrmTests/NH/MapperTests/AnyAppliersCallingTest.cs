using System;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.MapperTests
{
	public class AnyAppliersCallingTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
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
			orm.Setup(m => m.IsHeterogeneousAssociations(It.Is<MemberInfo>(p => p == typeof(MyClass).GetProperty("MyReferenceClass")))).Returns(true);
			return orm;
		}

		[Test]
		public void ApplierCalledPerSubclass()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);

			var applier = new Mock<IPatternApplier<MemberInfo, IAnyMapper>>();
			applier.Setup(x => x.Match(It.IsAny<MemberInfo>())).Returns(true);

			mapper.PatternsAppliers.Any.Add(applier.Object);
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			applier.Verify(x => x.Match(It.Is<MemberInfo>(mi => mi == typeof(MyClass).GetProperty("MyReferenceClass"))), Times.Once());
			applier.Verify(x => x.Apply(It.Is<MemberInfo>(mi => mi == typeof(MyClass).GetProperty("MyReferenceClass")), It.Is<IAnyMapper>(cm => cm != null)), Times.Once());
		}
	}
}