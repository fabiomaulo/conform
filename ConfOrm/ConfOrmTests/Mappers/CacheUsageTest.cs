using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class CacheUsageTest
	{
		[Test]
		public void ReadOnly()
		{
			CacheUsage.ReadOnly.Should().Not.Be.Null();
			CacheUsage.ReadOnly.ToHbm().Should().Be(HbmCacheUsage.ReadOnly);
		}

		[Test]
		public void ReadWrite()
		{
			CacheUsage.ReadWrite.Should().Not.Be.Null();
			CacheUsage.ReadWrite.ToHbm().Should().Be(HbmCacheUsage.ReadWrite);
		}

		[Test]
		public void NonstrictReadWrite()
		{
			CacheUsage.NonstrictReadWrite.Should().Not.Be.Null();
			CacheUsage.NonstrictReadWrite.ToHbm().Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}

		[Test]
		public void Transactional()
		{
			CacheUsage.Transactional.Should().Not.Be.Null();
			CacheUsage.Transactional.ToHbm().Should().Be(HbmCacheUsage.Transactional);
		}
	}
}