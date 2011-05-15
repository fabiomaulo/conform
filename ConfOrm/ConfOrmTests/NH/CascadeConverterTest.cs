using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class CascadeConverterTest
	{
		[Test]
		public void ConvertAll()
		{
			CascadeOn.All.ToCascadeString().Should().Contain("all");
		}

		[Test]
		public void ConvertPersist()
		{
			CascadeOn.Persist.ToCascadeString().Should().Contain("save-update").And.Contain("persist");
		}

		[Test]
		public void ConvertDeleteOrphans()
		{
			CascadeOn.DeleteOrphans.ToCascadeString().Should().Be.EqualTo("delete-orphan");
		}

		[Test]
		public void ConvertDetach()
		{
			CascadeOn.Detach.ToCascadeString().Should().Be.EqualTo("evict");
		}

		[Test]
		public void ConvertMerge()
		{
			CascadeOn.Merge.ToCascadeString().Should().Be.EqualTo("merge");
		}

		[Test]
		public void ConvertReAttach()
		{
			CascadeOn.ReAttach.ToCascadeString().Should().Be.EqualTo("lock");
		}

		[Test]
		public void ConvertRefresh()
		{
			CascadeOn.Refresh.ToCascadeString().Should().Be.EqualTo("refresh");
		}

		[Test]
		public void ConvertRemove()
		{
			CascadeOn.Remove.ToCascadeString().Should().Be.EqualTo("delete");
		}

		[Test]
		public void WhwnCombinationHasCommaSeparatedValues()
		{
			var cascadeString = (CascadeOn.Detach | CascadeOn.ReAttach).ToCascadeString();
			cascadeString.Should().Contain("evict").And.Contain("lock");
			cascadeString.Split(new[] { ',' }).Should().Have.SameValuesAs("lock", "evict");
		}
	}
}