using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers.UnionSubclassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class InheritedSimple : EntitySimple
		{
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Persister<UnionSubclassEntityPersister>();
			mapdoc.UnionSubclasses[0].Persister.Should().Contain("UnionSubclassEntityPersister");
		}
	}
}