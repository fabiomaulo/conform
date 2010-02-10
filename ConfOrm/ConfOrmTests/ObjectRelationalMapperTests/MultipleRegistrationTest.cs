using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class MultipleRegistrationTest
	{
		private class MyClass1
		{
			public int Id { get; set; }
		}

		private class MyClass2
		{
			public int Id { get; set; }
		}

		[Test]
		public void RegisterTablePerClass()
		{
			var entitiesTypes = new[] {typeof (MyClass1), typeof (MyClass2)};
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(entitiesTypes);
			orm.IsTablePerClass(typeof (MyClass1)).Should().Be(true);
			orm.IsTablePerClass(typeof(MyClass2)).Should().Be(true);

			orm.IsTablePerConcreteClass(typeof(MyClass1)).Should().Be(false);
			orm.IsTablePerClassHierarchy(typeof(MyClass2)).Should().Be(false);
		}

		[Test]
		public void RegisterTablePerClassHierarchy()
		{
			var entitiesTypes = new[] { typeof(MyClass1), typeof(MyClass2) };
			var orm = new ObjectRelationalMapper();
			orm.TablePerClassHierarchy(entitiesTypes);
			orm.IsTablePerClassHierarchy(typeof(MyClass1)).Should().Be(true);
			orm.IsTablePerClassHierarchy(typeof(MyClass2)).Should().Be(true);

			orm.IsTablePerConcreteClass(typeof(MyClass1)).Should().Be(false);
			orm.IsTablePerClass(typeof(MyClass2)).Should().Be(false);
		}

		[Test]
		public void RegisterTablePerConcreteClass()
		{
			var entitiesTypes = new[] { typeof(MyClass1), typeof(MyClass2) };
			var orm = new ObjectRelationalMapper();
			orm.TablePerConcreteClass(entitiesTypes);
			orm.IsTablePerConcreteClass(typeof(MyClass1)).Should().Be(true);
			orm.IsTablePerConcreteClass(typeof(MyClass2)).Should().Be(true);

			orm.IsTablePerClass(typeof(MyClass1)).Should().Be(false);
			orm.IsTablePerClassHierarchy(typeof(MyClass2)).Should().Be(false);
		}
	}
}