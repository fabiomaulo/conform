using System;
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
	public class PoidStrategyTest
	{
		private class EntityInt
		{
			public int Id { get; set; }
		}

		[Test]
		public void MapGeneratorUsingDomainInspectorInfo()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.HighLow, Params = new { max_low = 100, column = "NewHigh" } };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			mapping.RootClasses.Should().Have.Count.EqualTo(1);
			HbmClass rc = mapping.RootClasses.Single();
			var hbmGenerator = rc.Id.generator;
			hbmGenerator.@class.Should().Be.EqualTo("hilo");
			hbmGenerator.param.Select(p => p.name).Should().Have.SameValuesAs("max_low", "column");
			hbmGenerator.param.Select(p => p.GetText()).Should().Have.SameValuesAs("100", "NewHigh");
		}

		private Mock<IDomainInspector> GetBaseMock<T>()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(T)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(T)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			return orm;
		}

		[Test]
		public void MapGeneratorUsingHighLow()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.HighLow };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("hilo");
		}

		[Test]
		public void MapGeneratorUsingGuid()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.Guid };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("guid");
		}

		[Test]
		public void MapGeneratorUsingGuidComb()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.GuidOptimized };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("guid.comb");
		}

		[Test]
		public void MapGeneratorUsingSequence()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.Sequence };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("sequence");
		}

		[Test]
		public void MapGeneratorUsingIdentity()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.Identity };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("identity");
		}

		[Test]
		public void MapGeneratorUsingNative()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.Native };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.@class.Should().Be.EqualTo("native");
		}

		[Test]
		public void MapGeneratorUsingAssigned()
		{
			Mock<IDomainInspector> orm = GetBaseMock<EntityInt>();

			IPersistentIdStrategy idStrategy = new PoidStrategyStub { Strategy = PoIdStrategy.Assigned };
			orm.Setup(m => m.GetPersistentIdStrategy(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(idStrategy);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(EntityInt) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Id.generator.Should().Be.Null();
		}
	}

	public class PoidStrategyStub : IPersistentIdStrategy
	{
		#region Implementation of IPersistentIdStrategy

		public PoIdStrategy Strategy
		{
			get; set;
		}

		public object Params
		{
			get; set;
		}

		#endregion
	}
}