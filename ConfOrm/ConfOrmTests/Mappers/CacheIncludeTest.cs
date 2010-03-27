using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class CacheIncludeTest
	{
		[Test]
		public void All()
		{
			CacheInclude.All.Should().Not.Be.Null();
			CacheInclude.All.ToHbm().Should().Be(HbmCacheInclude.All);
		}

		[Test]
		public void NonLazy()
		{
			CacheInclude.NonLazy.Should().Not.Be.Null();
			CacheInclude.NonLazy.ToHbm().Should().Be(HbmCacheInclude.NonLazy);
		}
	}
}