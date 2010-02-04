using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class OneToOneMapperTest
	{
		[Test]
		public void AssignCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(hbm);
			mapper.Cascade(Cascade.Persist | Cascade.Remove);
			hbm.cascade.Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(hbm);
			mapper.Cascade(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Remove);
			hbm.cascade.Should().Not.Contain("orphans");
		}
	}
}