using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
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
			var hbmClass = new HbmClass();
			var nid = new NaturalIdMapper(typeof(EntitySimpleWithNaturalId), hbmClass, mapdoc);

			var hbmNaturalId = hbmClass.naturalid;
			nid.Mutable(true);
			hbmNaturalId.mutable.Should().Be.True();
		}
	}
}