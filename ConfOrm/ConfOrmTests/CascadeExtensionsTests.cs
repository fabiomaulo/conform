using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class CascadeExtensionsTests
	{
		[Test]
		public void IncludeShouldAddValue()
		{
			CascadeOn.All.Include(CascadeOn.DeleteOrphans).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
		}

		[Test]
		public void IncludeEveryButDeleteOrphansThenTranslateToAll()
		{
			(CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Merge | CascadeOn.Remove | CascadeOn.Detach).Include(CascadeOn.ReAttach).Should().Be(CascadeOn.All);
		}

		[Test]
		public void WhenHasAllThenDoesNotAddAlreadyIncluded()
		{
			CascadeOn.All.Include(CascadeOn.Merge).Should().Be(CascadeOn.All);
			CascadeOn.All.Include(CascadeOn.Persist).Should().Be(CascadeOn.All);
			CascadeOn.All.Include(CascadeOn.Detach).Should().Be(CascadeOn.All);
			CascadeOn.All.Include(CascadeOn.ReAttach).Should().Be(CascadeOn.All);
			CascadeOn.All.Include(CascadeOn.Refresh).Should().Be(CascadeOn.All);
			CascadeOn.All.Include(CascadeOn.Remove).Should().Be(CascadeOn.All);
		}

		[Test]
		public void WhenIncludeAllThenAll()
		{
			CascadeOn.Merge.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			CascadeOn.Persist.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			CascadeOn.Detach.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			CascadeOn.ReAttach.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			CascadeOn.Refresh.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			CascadeOn.Remove.Include(CascadeOn.All).Should().Be(CascadeOn.All);
			(CascadeOn.Remove | CascadeOn.Refresh).Include(CascadeOn.All).Should().Be(CascadeOn.All);
		}

		[Test]
		public void WhenSomethingAndDeleteOrphansAndIncludeAllThenAllDeleteOrphans()
		{
			(CascadeOn.Merge | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.Persist | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.Detach | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.ReAttach | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.Refresh | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.Remove | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
			(CascadeOn.Detach | CascadeOn.ReAttach | CascadeOn.DeleteOrphans).Include(CascadeOn.All).Should().Be(CascadeOn.All | CascadeOn.DeleteOrphans);
		}

		[Test]
		public void ExcludeShouldRemoveValue()
		{
			(CascadeOn.All | CascadeOn.DeleteOrphans).Exclude(CascadeOn.DeleteOrphans).Should().Be(CascadeOn.All);
			(CascadeOn.All | CascadeOn.DeleteOrphans).Exclude(CascadeOn.All).Should().Be(CascadeOn.DeleteOrphans);
		}

		[Test]
		public void WhenAllExcludeAllThenEqualNone()
		{
			CascadeOn.All.Exclude(CascadeOn.All).Should().Be(CascadeOn.None);
		}

		[Test]
		public void WhenExcludeFromAllThenReallyExcludeFromAll()
		{
			CascadeOn cascade = CascadeOn.All.Exclude(CascadeOn.Merge);
			cascade.Should().Be(CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Remove | CascadeOn.Detach | CascadeOn.ReAttach);
		}

		[Test]
		public void WhenExcludeFromAllDeleteOrphansThenReallyExcludeFromAllAndNotIgnoreDeleteOrphans()
		{
			(CascadeOn.All | CascadeOn.DeleteOrphans).Exclude(CascadeOn.Merge).Should().Be(CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Remove | CascadeOn.Detach | CascadeOn.ReAttach | CascadeOn.DeleteOrphans);
		}

		[Test]
		public void WhenExcledFromPartialThenExclude()
		{
			(CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Merge | CascadeOn.Remove | CascadeOn.Detach | CascadeOn.ReAttach).Exclude(CascadeOn.Detach)
				.Should().Be(CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Merge | CascadeOn.Remove | CascadeOn.ReAttach);
			(CascadeOn.Persist | CascadeOn.Merge | CascadeOn.DeleteOrphans | CascadeOn.Detach).Exclude(CascadeOn.Merge)
				.Should().Be(CascadeOn.Persist | CascadeOn.DeleteOrphans | CascadeOn.Detach);
		}
	}
}