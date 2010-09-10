namespace ConfOrm.Shop.Inflectors
{
	/// <summary>
	/// Inflector for pluralize and singularize Italian nouns. 
	/// </summary>
	/// <remarks>
	/// by Fabrizio Cornelli it-inflector@suppose.it
	/// </remarks>
	public class ItalianInflector : AbstractInflector
	{
		public ItalianInflector()
		{
			AddPlural("$", "i");
			AddPlural("a$", "e");
			AddPlural("o$", "i");
			AddPlural("e$", "i");
			AddPlural("i$", "i");
			AddPlural("um$", "a");
			AddPlural("io$", "i");

			AddPlural("co$", "chi");
			AddPlural("ca$", "che");
			AddPlural("go$", "ghi");
			AddPlural("ga$", "ghe");

			AddPlural("cio$", "ci");
			AddPlural("gio$", "gi");
			AddPlural("ico$", "ici");
			AddPlural("igo$", "igi");

			AddPlural("cia$", "ce");
			AddPlural("gia$", "ge");

			AddPlural("icia$", "icie");
			AddPlural("à$", "à");

			AddSingular("$", "");
			AddSingular("e$", "a");
			AddSingular("i$", "o");
			AddSingular("chi$", "co");
			AddSingular("che$", "ca");
			AddSingular("ghi$", "go");
			AddSingular("ghe$", "ga");

			AddIrregular("dilemma", "dilemmi");
			AddIrregular("medium", "media");
			AddIrregular("uomo", "uomini");
			AddIrregular("paio", "paia");
			AddIrregular("miglio", "miglia");
			AddIrregular("uovo", "uova");

			// these are not irregular, but there's no rule
			// to make singular. It depends by the gender of the word.
			AddIrregular("problema", "problemi");
			AddIrregular("inglese", "inglesi");
			AddIrregular("posizione", "posizioni");
			AddIrregular("referente", "referenti");
			AddIrregular("interruttore", "interruttori");
			AddIrregular("archivio", "archivi");
			AddIrregular("indice", "indici");
			AddIrregular("automobile", "automobili");
			AddIrregular("origine", "origini");

			AddUncountable("film");
			AddUncountable("computer");
			AddUncountable("murales");
			AddUncountable("attività");
			AddUncountable("bar");
			AddUncountable("radio");
			AddUncountable("moto");
			AddUncountable("personale");
		}
	}
}