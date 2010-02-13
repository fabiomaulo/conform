using System;
using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class PoidStrategyTest
	{
		private class EntityInt
		{
			public int Id { get; set; }
		}

		private class EntityGuid
		{
			public Guid Id { get; set; }
		}

		private class AnotherWithInt
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenIntPoidThenApplyHilo()
		{
			var orm = new ObjectRelationalMapper();
			orm.GetPersistentIdStrategy(typeof (EntityInt).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.HighLow);
		}

		[Test]
		public void WhenGuidPoidThenApplyGuidOptimized()
		{
			var orm = new ObjectRelationalMapper();
			orm.GetPersistentIdStrategy(typeof(EntityGuid).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.GuidOptimized);
		}

		[Test]
		public void WhenIntPoidExplicitIdentityThenApplyIdentity()
		{
			var orm = new ObjectRelationalMapper();
			orm.PoidStrategies.Add(new IdentityPoidPattern());
			orm.GetPersistentIdStrategy(typeof(EntityInt).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.Identity);
		}

		[Test]
		public void WhenIntPoidExplicitSequenceThenApplySequence()
		{
			var orm = new ObjectRelationalMapper();
			orm.PoidStrategies.Add(new SequencePoidPattern());
			orm.GetPersistentIdStrategy(typeof(EntityInt).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.Sequence);
		}

		[Test]
		public void WhenIntPoidExplicitNativeThenApplyNative()
		{
			var orm = new ObjectRelationalMapper();
			orm.PoidStrategies.Add(new NativePoidPattern());
			orm.GetPersistentIdStrategy(typeof(EntityInt).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.Native);
		}

		[Test]
		public void WhenGuidPoidExplicitGuidThenApplyGuid()
		{
			var orm = new ObjectRelationalMapper();
			orm.PoidStrategies.Add(new GuidPoidPattern());
			orm.GetPersistentIdStrategy(typeof(EntityGuid).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.Guid);
		}

		[Test]
		public void WhenExplicitIdentityForSpecificClassThenApplyOnlyInThatClassOnly()
		{
			var customPoidPattern = new Mock<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>>();
			var identityResult = new Mock<IPersistentIdStrategy>();
			identityResult.Setup(r => r.Strategy).Returns(PoIdStrategy.Identity);
			var specificId = typeof(AnotherWithInt).GetProperty("Id");
			customPoidPattern.Setup(p => p.Match(It.Is<MemberInfo>(mi => mi == specificId))).Returns(true);
			customPoidPattern.Setup(p => p.Get(It.Is<MemberInfo>(mi => mi == specificId))).Returns(identityResult.Object);
			var orm = new ObjectRelationalMapper();
			orm.PoidStrategies.Add(customPoidPattern.Object);
		
			orm.GetPersistentIdStrategy(typeof(EntityInt).GetProperty("Id")).Strategy.Should().Be.EqualTo(PoIdStrategy.HighLow);
			orm.GetPersistentIdStrategy(specificId).Strategy.Should().Be.EqualTo(PoIdStrategy.Identity);
		}
	}
}