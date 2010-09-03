namespace ConfOrm.UsageExamples.Packs
{
	public class Animal
	{
		public virtual long Id { get; set; }
		public virtual float BodyWeight { get; set; }
	}

	public class Mammal : Animal
	{
		public virtual bool Pregnant { get; set; }
	}

	public class Dog : Mammal
	{
	}

	public class Cat : Mammal
	{
	}
}