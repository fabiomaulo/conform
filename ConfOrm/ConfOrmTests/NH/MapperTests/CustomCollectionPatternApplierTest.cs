using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class CustomCollectionPatternApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<B> Bs { get; set; }
			public IDictionary<string, B> Bds { get; set; }
		}

		private class B
		{
			public int Id { get; set; }
		}

		[Test]
		public void AddCustomDelegatedApplier()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousCollectionApplierCount = mapper.PatternsAppliers.Collection.Count;

			mapper.AddCollectionPattern(mi => true, cm => cm.BatchSize(25));

			mapper.PatternsAppliers.Collection.Count.Should().Be(previousCollectionApplierCount + 1);
		}

		[Test]
		public void ExecuteCustomDelegatedApplierForBag()
		{
			Mock<IDomainInspector> orm = ArrangeDomainInspector();
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi.Name == "Bs"))).Returns(true);

			HbmMapping mapping = Act(orm);

			var hbmClass = mapping.RootClasses.Single();

			var hbmBag = (HbmBag)hbmClass.Properties.First(p => p.Name == "Bs");
			hbmBag.BatchSize.Should().Be(25);
		}

		[Test]
		public void ExecuteCustomDelegatedApplierForSet()
		{
			Mock<IDomainInspector> orm = ArrangeDomainInspector();
			orm.Setup(m => m.IsSet(It.Is<MemberInfo>(mi => mi.Name == "Bs"))).Returns(true);

			HbmMapping mapping = Act(orm);

			var hbmClass = mapping.RootClasses.Single();

			var hbmSet = (HbmSet)hbmClass.Properties.First(p => p.Name == "Bs");
			hbmSet.BatchSize.Should().Be(25);
		}

		[Test]
		public void ExecuteCustomDelegatedApplierForList()
		{
			Mock<IDomainInspector> orm = ArrangeDomainInspector();
			orm.Setup(m => m.IsList(It.Is<MemberInfo>(mi => mi.Name == "Bs"))).Returns(true);

			HbmMapping mapping = Act(orm);

			var hbmClass = mapping.RootClasses.Single();
			var hbmSet = (HbmList)hbmClass.Properties.First(p => p.Name == "Bs");
			hbmSet.BatchSize.Should().Be(25);
		}

		[Test]
		public void ExecuteCustomDelegatedApplierForDictionary()
		{
			Mock<IDomainInspector> orm = ArrangeDomainInspector();
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi.Name == "Bds"))).Returns(true);

			HbmMapping mapping = Act(orm);

			var hbmClass = mapping.RootClasses.Single();
			var hbmMap = (HbmMap)hbmClass.Properties.First(p => p.Name == "Bds");
			hbmMap.BatchSize.Should().Be(25);
		}

		private HbmMapping Act(Mock<IDomainInspector> orm)
		{
			var mapper = new Mapper(orm.Object);
			mapper.AddCollectionPattern(mi => true, cm => cm.BatchSize(25));
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		private Mock<IDomainInspector> ArrangeDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void AddCustomDelegatedApplierWithMember()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousCollectionApplierCount = mapper.PatternsAppliers.Collection.Count;

			mapper.AddCollectionPattern(mi => true, (mi, cm) => cm.BatchSize(25));

			mapper.PatternsAppliers.Collection.Count.Should().Be(previousCollectionApplierCount + 1);
		}
	}
}