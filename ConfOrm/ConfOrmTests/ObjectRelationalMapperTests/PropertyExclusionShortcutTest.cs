using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class PropertyExclusionShortcutTest
	{
		public class Person
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
		}

		public class Student : Person
		{
			public virtual string Grade { get; set; }
			public virtual string Class { get; set; }
		}

		public class Clerk : Person
		{
			public virtual string Company { get; set; }
		}
		public class OutSideHierarchy
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
		}

		[Test]
		public void WhenCalledShouldAddPersistentPropertiesExclusionsPattern()
		{
			var orm = new ObjectRelationalMapper();
			var oldCount = orm.Patterns.PersistentPropertiesExclusions.Count;
			orm.ExcludeProperty(mi=> mi.DeclaringType != typeof(Person));
			orm.Patterns.PersistentPropertiesExclusions.Count.Should().Be(oldCount + 1);
		}

		[Test]
		public void AskForPersistentPropertyShouldUsePersistentPropertiesExclusionsPattern()
		{
			// This test look like an example and a test related to the predicate passed to orm.ExcludeProperty ; btw match the scope.
			var orm = new ObjectRelationalMapper();
			orm.ExcludeProperty(mi => mi.DeclaringType != typeof(Person) && typeof(Person).IsAssignableFrom(mi.DeclaringType));
			orm.IsPersistentProperty(ForClass<Person>.Property(x => x.Id)).Should().Be.True();
			orm.IsPersistentProperty(ForClass<Person>.Property(x => x.Name)).Should().Be.True();
			orm.IsPersistentProperty(ForClass<Student>.Property(x => x.Grade)).Should().Be.False();
			orm.IsPersistentProperty(ForClass<Student>.Property(x => x.Class)).Should().Be.False();
			orm.IsPersistentProperty(ForClass<Clerk>.Property(x => x.Company)).Should().Be.False();
			orm.IsPersistentProperty(ForClass<OutSideHierarchy>.Property(x => x.Id)).Should().Be.True();
			orm.IsPersistentProperty(ForClass<OutSideHierarchy>.Property(x => x.Name)).Should().Be.True();
		}
	}
}