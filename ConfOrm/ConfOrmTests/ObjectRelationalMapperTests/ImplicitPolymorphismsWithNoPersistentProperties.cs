using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ImplicitPolymorphismsWithNoPersistentProperties
	{
		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private class Entity : BaseEntity
		{
			public bool IsValid { get { return false; } }
			public string Something { get; set; }
		}

		private class Person : Entity
		{
			public string Name { get; set; }
		}

		[Test]
		public void WhenNotRegisterPersistentPropertyThenShouldMapOnlyPersistentProperties()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] {typeof (Person)});

			var hbmClass = mappings.RootClasses.Single();
			hbmClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Something", "Name");
		}

		[Test]
		public void WhenRegisterPersistentPropertyOnExplicitEntityThenShouldMapEvenPersistentReadOnlyProperties()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.PersistentProperty<Person>(p => p.IsValid);

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Person) });

			var hbmClass = mappings.RootClasses.Single();
			hbmClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Something", "Name", "IsValid");
		}

		[Test]
		public void WhenRegisterPersistentPropertyOnImplicitEntityThenShouldMapEvenPersistentReadOnlyProperties()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.PersistentProperty<Entity>(p => p.IsValid);

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Person) });

			var hbmClass = mappings.RootClasses.Single();
			hbmClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Something", "Name", "IsValid");
		}
	}
}