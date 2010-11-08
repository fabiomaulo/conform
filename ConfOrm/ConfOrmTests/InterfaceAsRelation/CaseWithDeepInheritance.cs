using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class CaseWithDeepInheritance
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
		public void WhenComplexInheritanceSho()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Animal>();
			orm.ManyToMany<Human, Human>();
			orm.AddToDomain(new[] { typeof(Animal), typeof(Reptile), typeof(Lizard), typeof(Mammal), typeof(Human), typeof(DomesticAnimal), typeof(Cat), typeof(Dog)});
			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Animal)});

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Animal"));
			rc.Properties.Where(p => p.Name == "Mother").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Be.Null();
			rc.Properties.Where(p => p.Name == "Father").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Be.Null();
		}

		[Test]
		public void WhenCircularRelationThenIsManyToOneAndNotHeterogeneous()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Animal>();
			orm.ManyToMany<Human, Human>();
			orm.AddToDomain(new[] { typeof(Animal), typeof(Reptile), typeof(Lizard), typeof(Mammal), typeof(Human), typeof(DomesticAnimal), typeof(Cat), typeof(Dog) });
			orm.IsManyToOne(typeof(Animal), typeof(Animal)).Should().Be.True();
			orm.IsHeterogeneousAssociation(ForClass<Animal>.Property(x=> x.Mother)).Should().Be.False();
			orm.IsHeterogeneousAssociation(ForClass<Animal>.Property(x => x.Father)).Should().Be.False();
		}
	}
}