using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class InterfaceImplicitPolymorphismsWithNoPersistentProperties
	{
		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private interface IEntity
		{
			bool IsValid { get; }
			string Something { get; set; }
		}

		private class Person : BaseEntity, IEntity
		{
			public string Name { get; set; }
			public bool IsValid { get { return false; }}
			public string Something { get; set; }
		}

		[Test]
		public void WhenRegisterPersistentPropertyOnInterfaceThenShouldRecognizePropertyOfConcreteImpl()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.PersistentProperty<IEntity>(p => p.IsValid);

			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.IsValid)).Should().Be.True();
		}

		[Test]
		public void WhenRegisterExclusionPropertyOnInterfaceThenShouldExcludePropertyOfConcreteImpl()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.ExcludeProperty<IEntity>(p => p.Something);

			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.Something)).Should().Be.False();
		}

		[Test]
		public void WhenRegisterExclusionPropertyOnInterfaceAndInclusionOnConcreteThenShouldIncludePropertyOfConcreteImpl()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			orm.ExcludeProperty<IEntity>(p => p.Something);
			orm.PersistentProperty<Person>(p => p.Something);

			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.Something)).Should().Be.True();
		}

		[Test]
		public void WhenNoRegisterExclusionPropertyForInterfaceThenShouldWorkNormally()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();

			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.Something)).Should().Be.True();
			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.IsValid)).Should().Be.False();
			orm.IsPersistentProperty(ForClass<Person>.Property(p => p.Name)).Should().Be.True();
		}
	}
}