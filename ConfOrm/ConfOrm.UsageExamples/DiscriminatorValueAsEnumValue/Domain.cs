namespace ConfOrm.UsageExamples.DiscriminatorValueAsEnumValue
{
	public class Item
	{
		public int Id { get; set; }
	}

	public class Post : Item { }

	public class Contribute : Post { }

	public class Page : Item { }

	public class Gallery : Item { }

	// Another hierarchy where apply "default" pattern
	public class Something
	{
		public int Id { get; set; }
	}
	public class SomethingElse : Something { }
}