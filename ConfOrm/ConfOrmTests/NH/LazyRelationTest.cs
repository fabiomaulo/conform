using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class LazyRelationTest
	{
		[Test]
		public void WhenProxyThenProxy()
		{
			LazyRelation.Proxy.ToHbm().Should().Be(HbmLaziness.Proxy);
		}

		[Test]
		public void WhenNoProxyThenNoProxy()
		{
			LazyRelation.NoProxy.ToHbm().Should().Be(HbmLaziness.NoProxy);
		}

		[Test]
		public void WhenNoLazyThenFalse()
		{
			LazyRelation.NoLazy.ToHbm().Should().Be(HbmLaziness.False);
		}
	}
}