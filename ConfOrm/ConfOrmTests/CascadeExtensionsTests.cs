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
		public void ExcludeShouldRemoveValue()
		{
			(Cascade.All | Cascade.DeleteOrphans).Exclude(Cascade.DeleteOrphans).Should().Be(Cascade.All);
		}
	}
}