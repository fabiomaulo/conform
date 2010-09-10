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
			"ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞ‗אבגדהוזחטיךכלםמןנסעףפץצרשתûü‎‏ÿ".Unaccent()
				.Should().Be.EqualTo("AAAAAAACEEEEIIIIDNOOOOOOUUUUYTsaaaaaaaceeeeiiiienoooooouuuuyty");
		}
	}
}