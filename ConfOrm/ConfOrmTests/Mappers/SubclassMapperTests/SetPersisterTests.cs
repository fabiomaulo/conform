using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers.SubclassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class HineritedSimple: EntitySimple
		{
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new SubclassMapper(typeof(HineritedSimple), mapdoc);
			rc.Persister<SingleTableEntityPersister>();
			mapdoc.SubClasses[0].Persister.Should().Contain("SingleTableEntityPersister");
		}
	}
}