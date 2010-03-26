using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class NaturalIdMapperTest
	{
		private class EntitySimpleWithNaturalId
		{
		}

		[Test]
		public void CanSetMutable()
		{
			var mapdoc = new HbmMapping();
			var hbmNaturalId = new HbmNaturalId();
			var nid = new NaturalIdMapper(typeof(EntitySimpleWithNaturalId), hbmNaturalId, mapdoc);

			nid.Mutable(true);
			hbmNaturalId.mutable.Should().Be.True();
		}
	}
}