using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class CascadeConverterTest
	{
		[Test]
		public void ConvertAll()
		{
			Cascade.All.ToCascadeString().Should().Contain("all");
		}

		[Test]
		public void ConvertPersist()
		{
			Cascade.Persist.ToCascadeString().Should().Contain("save-update").And.Contain("persist");
		}

		[Test]
		public void ConvertDeleteOrphans()
		{
			Cascade.DeleteOrphans.ToCascadeString().Should().Be.EqualTo("delete-orphan");
		}

		[Test]
		public void ConvertDetach()
		{
			Cascade.Detach.ToCascadeString().Should().Be.EqualTo("evict");
		}

		[Test]
		public void ConvertMerge()
		{
			Cascade.Merge.ToCascadeString().Should().Be.EqualTo("merge");
		}

		[Test]
		public void ConvertReAttach()
		{
			Cascade.ReAttach.ToCascadeString().Should().Be.EqualTo("lock");
		}

		[Test]
		public void ConvertRefresh()
		{
			Cascade.Refresh.ToCascadeString().Should().Be.EqualTo("refresh");
		}

		[Test]
		public void ConvertRemove()
		{
			Cascade.Remove.ToCascadeString().Should().Be.EqualTo("delete");
		}

		[Test]
		public void WhwnCombinationHasCommaSeparatedValues()
		{
			var cascadeString = (Cascade.Detach | Cascade.ReAttach).ToCascadeString();
			cascadeString.Should().Contain("evict").And.Contain("lock");
			cascadeString.Split(new[] { ',' }).Should().Have.SameValuesAs("lock", "evict");
		}
	}
}