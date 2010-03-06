using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class OneToOneMapperTest
	{
		private class MyClass
		{
			public Relation Relation { get; set; }
		}

		private class Relation
		{

		}

		[Test]
		public void AssignCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Cascade(Cascade.Persist | Cascade.Remove);
			hbm.cascade.Split(',').Select(w=> w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Cascade(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w=> w.Satisfy(cascade=> !cascade.Contains("orphan")));
		}

		[Test]
		public void CanSetAccessor()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Lazy(LazyRelation.NoProxy);
			hbm.Lazy.Should().Have.Value();
			hbm.Lazy.Should().Be(HbmLaziness.NoProxy);
		}
	}
}