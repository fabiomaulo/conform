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

		[Test]
		public void SplitWords()
		{
			"OrdenCliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", "Cliente" });
			"Orden_Cliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", "_", "Cliente" });
			"Orden Cliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", " ", "Cliente" });
			"Orig�nOrd�n".SplitWords().Should().Have.SameSequenceAs(new[] { "Orig�n", "Ord�n" });
		}
	}
}