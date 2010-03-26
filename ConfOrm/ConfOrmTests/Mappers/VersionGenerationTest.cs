using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class VersionGenerationTest
	{
		[Test]
		public void FieldNeverShouldNotBeNull()
		{
			VersionGeneration.Never.Should().Not.Be.Null();
		}

		[Test]
		public void FieldAlwaysShouldNotBeNull()
		{
			VersionGeneration.Always.Should().Not.Be.Null();
		}

		[Test]
		public void NeverShouldReturnHbmVersionGeneration()
		{
			VersionGeneration.Never.ToHbm().Should().Be(HbmVersionGeneration.Never);
		}

		[Test]
		public void AlwaysShouldReturnHbmVersionGeneration()
		{
			VersionGeneration.Always.ToHbm().Should().Be(HbmVersionGeneration.Always);
		}
	}
}