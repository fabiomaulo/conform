using ConfOrm.Shop.Inflectors;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorsTests
{
	public class EnglishInflectorTest: CommonInflectorTests
	{
		/// Originally implemented by http://andrewpeters.net/inflectornet/
		public EnglishInflectorTest()
		{
			SingularToPlural.Add("search", "searches");
			SingularToPlural.Add("switch", "switches");
			SingularToPlural.Add("fix", "fixes");
			SingularToPlural.Add("box", "boxes");
			SingularToPlural.Add("process", "processes");
			SingularToPlural.Add("address", "addresses");
			SingularToPlural.Add("case", "cases");
			SingularToPlural.Add("stack", "stacks");
			SingularToPlural.Add("wish", "wishes");
			SingularToPlural.Add("fish", "fish");

			SingularToPlural.Add("category", "categories");
			SingularToPlural.Add("query", "queries");
			SingularToPlural.Add("ability", "abilities");
			SingularToPlural.Add("agency", "agencies");
			SingularToPlural.Add("movie", "movies");

			SingularToPlural.Add("archive", "archives");

			SingularToPlural.Add("index", "indices");

			SingularToPlural.Add("wife", "wives");
			SingularToPlural.Add("safe", "saves");
			SingularToPlural.Add("half", "halves");

			SingularToPlural.Add("move", "moves");

			SingularToPlural.Add("salesperson", "salespeople");
			SingularToPlural.Add("person", "people");

			SingularToPlural.Add("spokesman", "spokesmen");
			SingularToPlural.Add("man", "men");
			SingularToPlural.Add("woman", "women");

			SingularToPlural.Add("basis", "bases");
			SingularToPlural.Add("diagnosis", "diagnoses");

			SingularToPlural.Add("datum", "data");
			SingularToPlural.Add("medium", "media");
			SingularToPlural.Add("analysis", "analyses");

			SingularToPlural.Add("node_child", "node_children");
			SingularToPlural.Add("child", "children");

			SingularToPlural.Add("experience", "experiences");
			SingularToPlural.Add("day", "days");

			SingularToPlural.Add("comment", "comments");
			SingularToPlural.Add("foobar", "foobars");
			SingularToPlural.Add("newsletter", "newsletters");

			SingularToPlural.Add("old_news", "old_news");
			SingularToPlural.Add("news", "news");

			SingularToPlural.Add("series", "series");
			SingularToPlural.Add("species", "species");

			SingularToPlural.Add("quiz", "quizzes");

			SingularToPlural.Add("perspective", "perspectives");

			SingularToPlural.Add("ox", "oxen");
			SingularToPlural.Add("photo", "photos");
			SingularToPlural.Add("buffalo", "buffaloes");
			SingularToPlural.Add("tomato", "tomatoes");
			SingularToPlural.Add("dwarf", "dwarves");
			SingularToPlural.Add("elf", "elves");
			SingularToPlural.Add("information", "information");
			SingularToPlural.Add("equipment", "equipment");
			SingularToPlural.Add("bus", "buses");
			SingularToPlural.Add("status", "statuses");
			SingularToPlural.Add("status_code", "status_codes");
			SingularToPlural.Add("mouse", "mice");

			SingularToPlural.Add("louse", "lice");
			SingularToPlural.Add("house", "houses");
			SingularToPlural.Add("octopus", "octopi");
			SingularToPlural.Add("virus", "viri");
			SingularToPlural.Add("alias", "aliases");
			SingularToPlural.Add("portfolio", "portfolios");

			SingularToPlural.Add("vertex", "vertices");
			SingularToPlural.Add("matrix", "matrices");

			SingularToPlural.Add("axis", "axes");
			SingularToPlural.Add("testis", "testes");
			SingularToPlural.Add("crisis", "crises");

			SingularToPlural.Add("rice", "rice");
			SingularToPlural.Add("shoe", "shoes");

			SingularToPlural.Add("horse", "horses");
			SingularToPlural.Add("prize", "prizes");
			SingularToPlural.Add("edge", "edges");

			TestInflector = new EnglishInflector();
		}

		[Test]
		public void PluralizePlurals()
		{
			TestInflector.Pluralize("plurals").Should().Be("plurals");
			TestInflector.Pluralize("Plurals").Should().Be("Plurals");
		}
	}
}