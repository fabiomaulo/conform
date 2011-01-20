using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers.ClassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, ForClass<EntitySimple>.Property(x => x.Id));
			rc.Persister<SingleTableEntityPersister>();
			mapdoc.RootClasses[0].Persister.Should().Contain("SingleTableEntityPersister");
		}
	}
}