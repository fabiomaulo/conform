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
			"ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏĞÑÒÓÔÕÖØÙÚÛÜİŞßàáâãäåæçèéêëìíîïğñòóôõöøùúûüışÿ".Unaccent()
				.Should().Be.EqualTo("AAAAAAACEEEEIIIIDNOOOOOOUUUUYTsaaaaaaaceeeeiiiienoooooouuuuyty");
		}

		[Test]
		public void SplitWords()
		{
			"OrdenCliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", "Cliente" });
			"Orden_Cliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", "_", "Cliente" });
			"Orden Cliente".SplitWords().Should().Have.SameSequenceAs(new[] { "Orden", " ", "Cliente" });
			"OrigénOrdén".SplitWords().Should().Have.SameSequenceAs(new[] { "Origén", "Ordén" });
		}
	}
}