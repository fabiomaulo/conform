using ConfOrm.Shop.Inflectors;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorsTests
{
	public class InflectorExtensionsTests
	{
		[Test]
		public void Unaccent()
		{
			"��������������������������������������������������������������".Unaccent()
				.Should().Be.EqualTo("AAAAAAACEEEEIIIIDNOOOOOOUUUUYTsaaaaaaaceeeeiiiienoooooouuuuyty");
		}
	}
}