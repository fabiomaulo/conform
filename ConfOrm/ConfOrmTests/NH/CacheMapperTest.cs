using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class CacheMapperTest
	{
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new CacheMapper(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCreatedThenAutoAssignUsage()
		{
			var hbmCache = new HbmCache();
			new CacheMapper(hbmCache);
			hbmCache.usage.Should().Be(HbmCacheUsage.Transactional);
		}

		[Test]
		public void CanSetUsage()
		{
			var hbmCache = new HbmCache();
			var mapper = new CacheMapper(hbmCache);
			mapper.Usage(CacheUsage.ReadWrite);
			hbmCache.usage.Should().Be(HbmCacheUsage.ReadWrite);
		}

		[Test]
		public void CanSetRegion()
		{
			var hbmCache = new HbmCache();
			var mapper = new CacheMapper(hbmCache);
			mapper.Region("pizza");
			hbmCache.region.Should().Be("pizza");
		}

		[Test]
		public void CanSetInclude()
		{
			var hbmCache = new HbmCache();
			var mapper = new CacheMapper(hbmCache);
			mapper.Include(CacheInclude.NonLazy);
			hbmCache.include.Should().Be(HbmCacheInclude.NonLazy);
		}
	}
}