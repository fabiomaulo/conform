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
			Cascade.All.Include(Cascade.DeleteOrphans).Should().Be(Cascade.All | Cascade.DeleteOrphans);
		}

		[Test]
		public void IncludeEveryButDeleteOrphansThenTranslateToAll()
		{
			(Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove | Cascade.Detach).Include(Cascade.ReAttach).Should().Be(Cascade.All);
		}

		[Test]
		public void WhenHasAllThenDoesNotAddAlreadyIncluded()
		{
			Cascade.All.Include(Cascade.Merge).Should().Be(Cascade.All);
			Cascade.All.Include(Cascade.Persist).Should().Be(Cascade.All);
			Cascade.All.Include(Cascade.Detach).Should().Be(Cascade.All);
			Cascade.All.Include(Cascade.ReAttach).Should().Be(Cascade.All);
			Cascade.All.Include(Cascade.Refresh).Should().Be(Cascade.All);
			Cascade.All.Include(Cascade.Remove).Should().Be(Cascade.All);
		}

		[Test]
		public void WhenIncludeAllThenAll()
		{
			Cascade.Merge.Include(Cascade.All).Should().Be(Cascade.All);
			Cascade.Persist.Include(Cascade.All).Should().Be(Cascade.All);
			Cascade.Detach.Include(Cascade.All).Should().Be(Cascade.All);
			Cascade.ReAttach.Include(Cascade.All).Should().Be(Cascade.All);
			Cascade.Refresh.Include(Cascade.All).Should().Be(Cascade.All);
			Cascade.Remove.Include(Cascade.All).Should().Be(Cascade.All);
			(Cascade.Remove | Cascade.Refresh).Include(Cascade.All).Should().Be(Cascade.All);
		}

		[Test]
		public void WhenSomethingAndDeleteOrphansAndIncludeAllThenAllDeleteOrphans()
		{
			(Cascade.Merge | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.Persist | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.Detach | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.ReAttach | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.Refresh | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.Remove | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
			(Cascade.Detach | Cascade.ReAttach | Cascade.DeleteOrphans).Include(Cascade.All).Should().Be(Cascade.All | Cascade.DeleteOrphans);
		}

		[Test]
		public void ExcludeShouldRemoveValue()
		{
			(Cascade.All | Cascade.DeleteOrphans).Exclude(Cascade.DeleteOrphans).Should().Be(Cascade.All);
			(Cascade.All | Cascade.DeleteOrphans).Exclude(Cascade.All).Should().Be(Cascade.DeleteOrphans);
		}

		[Test]
		public void WhenAllExcludeAllThenEqualNone()
		{
			Cascade.All.Exclude(Cascade.All).Should().Be(Cascade.None);
		}

		[Test]
		public void WhenExcludeFromAllThenReallyExcludeFromAll()
		{
			Cascade cascade = Cascade.All.Exclude(Cascade.Merge);
			cascade.Should().Be(Cascade.Persist | Cascade.Refresh | Cascade.Remove | Cascade.Detach | Cascade.ReAttach);
		}

		[Test]
		public void WhenExcludeFromAllDeleteOrphansThenReallyExcludeFromAllAndNotIgnoreDeleteOrphans()
		{
			(Cascade.All | Cascade.DeleteOrphans).Exclude(Cascade.Merge).Should().Be(Cascade.Persist | Cascade.Refresh | Cascade.Remove | Cascade.Detach | Cascade.ReAttach | Cascade.DeleteOrphans);
		}

		[Test]
		public void WhenExcledFromPartialThenExclude()
		{
			(Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove | Cascade.Detach | Cascade.ReAttach).Exclude(Cascade.Detach)
				.Should().Be(Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove | Cascade.ReAttach);
			(Cascade.Persist | Cascade.Merge | Cascade.DeleteOrphans | Cascade.Detach).Exclude(Cascade.Merge)
				.Should().Be(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Detach);
		}
	}
}