using ConfOrm.Shop.Inflectors;
using NUnit.Framework;

namespace ConfOrm.ShopTests.InflectorsTests
{
	[TestFixture]
	public class ItalianInflectorTest : CommonInflectorTests
	{
		public ItalianInflectorTest()
		{
			SingularToPlural.Add("inglese", "inglesi");
			SingularToPlural.Add("prova", "prove");
			SingularToPlural.Add("veicolo", "veicoli");
			SingularToPlural.Add("dispositivo", "dispositivi");
			SingularToPlural.Add("posizione", "posizioni");
			SingularToPlural.Add("attività", "attività");
			SingularToPlural.Add("intervento", "interventi");
			SingularToPlural.Add("personale", "personale");
			SingularToPlural.Add("referente", "referenti");
			SingularToPlural.Add("città", "città");
			SingularToPlural.Add("camicia", "camicie");
			SingularToPlural.Add("uomo", "uomini");
			SingularToPlural.Add("sacco", "sacchi");
			SingularToPlural.Add("lago", "laghi");
			SingularToPlural.Add("amico", "amici");
			SingularToPlural.Add("paio", "paia");
			SingularToPlural.Add("miglio", "miglia");
			SingularToPlural.Add("uovo", "uova");

			SingularToPlural.Add("decreto", "decreti");

			SingularToPlural.Add("ricerca", "ricerche");
			SingularToPlural.Add("interruttore", "interruttori");
			SingularToPlural.Add("processo", "processi");
			SingularToPlural.Add("indirizzo", "indirizzi");

			SingularToPlural.Add("categoria", "categorie");
			SingularToPlural.Add("domanda", "domande");
			SingularToPlural.Add("abilità", "abilità");
			SingularToPlural.Add("agenzia", "agenzie");
			SingularToPlural.Add("film", "film");
			SingularToPlural.Add("archivio", "archivi");
			SingularToPlural.Add("indice", "indici");

			SingularToPlural.Add("automobile", "automobili");

			SingularToPlural.Add("Origine", "Origini");
			SingularToPlural.Add("Fattura", "Fatture");

			SingularToPlural.Add("Attività", "Attività");
			SingularToPlural.Add("Telefono", "Telefoni");

			TestInflector = new ItalianInflector();
		}
	}
}