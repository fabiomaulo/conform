using ConfOrm.Mappers;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class UseNoLazyForNoProxableApplierTest
	{
		public class MyNoProxableRoot
		{
			public int Something { get; set; }
		}

		public class MyNoProxableSub: MyNoProxableRoot
		{
			public int SomethingElse { get; set; }
		}

		public class MyProxableRoot
		{
			public virtual int Something { get; set; }
		}

		public class MyProxableSub : MyProxableRoot
		{
			public virtual int SomethingElse { get; set; }
		}

		[Test]
		public void WhenNoProxableRootThenApplyLazyFalse()
		{
			var applier = new UseNoLazyForNoProxableApplier<IClassMapper>();
			var mapper = new Mock<IClassMapper>();

			applier.Match(typeof(MyNoProxableRoot)).Should().Be.True();
			applier.Apply(typeof(MyNoProxableRoot), mapper.Object);

			mapper.Verify(x=> x.Lazy(false));
		}

		[Test]
		public void WhenNoProxableSubThenApplyLazyFalse()
		{
			var applier = new UseNoLazyForNoProxableApplier<ISubclassMapper>();
			var mapper = new Mock<ISubclassMapper>();

			applier.Match(typeof(MyNoProxableSub)).Should().Be.True();
			applier.Apply(typeof(MyNoProxableSub), mapper.Object);

			mapper.Verify(x => x.Lazy(false));
		}

		[Test]
		public void WhenProxableRootThenNoMatch()
		{
			var applier = new UseNoLazyForNoProxableApplier<IClassMapper>();

			applier.Match(typeof(MyProxableRoot)).Should().Be.False();
		}

		[Test]
		public void WhenProxableSubThenNoMatch()
		{
			var applier = new UseNoLazyForNoProxableApplier<ISubclassMapper>();

			applier.Match(typeof(MyProxableSub)).Should().Be.False();
		}
	}
}