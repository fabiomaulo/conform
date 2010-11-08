using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.PolymorphismRelationsTests
{
	public class CaseWithComplexHierarchyTest
	{
		private class Animal
		{
			public virtual long Id { get; set; }

			public virtual ISet<Animal> Offspring { get; set; }

			public virtual Animal Mother { get; set; }

			public virtual Animal Father { get; set; }

			public virtual void AddOffspring(Animal offSpring)
			{
				if (Offspring == null)
				{
					Offspring = new HashedSet<Animal>();
				}

				Offspring.Add(offSpring);
			}
		}

		private class Reptile : Animal
		{
			public virtual float BodyTemperature { get; set; }
		}

		private class Lizard : Reptile { }
		private class Mammal : Animal
		{
			public Mammal()
			{
				Birthdate = DateTime.Today;
			}

			public virtual bool Pregnant { get; set; }

			public virtual DateTime Birthdate { get; set; }
		}
		private class Human : Mammal
		{
			public virtual ICollection<Human> Friends { get; set; }

			public virtual ICollection<DomesticAnimal> Pets { get; set; }
		}

		private class DomesticAnimal : Mammal
		{
			public virtual Human Owner { get; set; }
		}

		private class Cat : DomesticAnimal { }

		private class Dog : DomesticAnimal { }

		[Test]
		public void GetBaseImplementorsShouldReturnOnlyTheFirstBaseClassOfTheHierarchy()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Animal>();
			orm.ManyToMany<Human, Human>();
			orm.AddToDomain(new[] { typeof(Animal), typeof(Reptile), typeof(Lizard), typeof(Mammal), typeof(Human), typeof(DomesticAnimal), typeof(Cat), typeof(Dog) });
			orm.GetBaseImplementors(typeof(Animal)).Should().Have.SameValuesAs(new[] { typeof(Animal) });
		}
	}
}