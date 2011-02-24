namespace ConfOrm.UsageExamples.HighLowPerEntity
{
	public abstract class Product
	{
		public virtual int Id { get; set; }
		public virtual string Title { get; set; }
	}

	public class Book: Product
	{
		public virtual string Author { get; set; }
	}

	public class Movie : Product
	{
		public virtual string Actor { get; set; }
	}

	public class Customer
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}