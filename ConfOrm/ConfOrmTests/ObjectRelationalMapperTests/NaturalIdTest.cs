using System;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class NaturalIdTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public Related Related { get; set; }
			public MyComponent MyComponent { get; set; }
			public object Any { get; set; }
		}

		private class Related
		{
			public int Id { get; set; }
		}

		private class MyComponent
		{
			public string FirstName { get; set; }
		}

		[Test]
		public void WhenDefineWithoutRootEntityThenThrow()
		{
			var orm = new ObjectRelationalMapper();
			Executing.This(() => orm.NaturalId<MyClass>(x=> x.Any)).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenDefineWithoutPropertiesThenNothingHappen()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			Executing.This(() => orm.NaturalId<MyClass>()).Should().NotThrow();
		}

		[Test]
		public void WhenDefineRootEntityThenRegister()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.TablePerClass<Related>();
			orm.NaturalId<MyClass>(x => x.Name, x => x.Related, x => x.MyComponent, x => x.Any);

			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Name)).Should().Be.True();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Related)).Should().Be.True();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.MyComponent)).Should().Be.True();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Any)).Should().Be.True();
		}

		[Test]
		public void WhenDefineWithNullThenRegisterValid()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			orm.TablePerClass<Related>();
			orm.NaturalId<MyClass>(x => x.Name, null, x => x.MyComponent, null);

			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Name)).Should().Be.True();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Related)).Should().Be.False();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.MyComponent)).Should().Be.True();
			orm.IsMemberOfNaturalId(ForClass<MyClass>.Property(x => x.Any)).Should().Be.False();
		}
	}
}