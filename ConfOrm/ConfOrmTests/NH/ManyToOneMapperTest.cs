using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ManyToOneMapperTest
	{
		[Test]
		public void AssignCascadeStyle()
		{
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(hbm);
			mapper.Cascade(Cascade.Persist | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(hbm);
			mapper.Cascade(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w => w.Satisfy(cascade => !cascade.Contains("orphan")));
		}
	}
}