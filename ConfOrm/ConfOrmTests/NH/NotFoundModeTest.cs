using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class NotFoundModeTest
	{
		[Test]
		public void WhenProxyThenProxy()
		{
			NotFoundMode.Ignore.ToHbm().Should().Be(HbmNotFoundMode.Ignore);
		}

		[Test]
		public void WhenNoProxyThenNoProxy()
		{
			NotFoundMode.Exception.ToHbm().Should().Be(HbmNotFoundMode.Exception);
		}
	}
}