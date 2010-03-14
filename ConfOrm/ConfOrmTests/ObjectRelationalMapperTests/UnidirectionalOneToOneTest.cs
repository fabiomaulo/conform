using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class UnidirectionalOneToOneTest
	{
		// a unidirectional One-To-One should be mapped as a many-to-one
		private class MyClass
		{
			public int Id { get; set; }
			public MyOneClass MyOneClass { get; set; }
		}
		private class MyOneClass
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenUnidirectionalOneToOneThenShouldBeManyToOne()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(new[] {typeof (MyClass), typeof (MyOneClass)});
			orm.OneToOne<MyClass, MyOneClass>();

			orm.IsOneToOne(typeof (MyClass), typeof (MyOneClass)).Should().Be.False();
			orm.IsManyToOne(typeof(MyClass), typeof(MyOneClass)).Should().Be.True();
			orm.IsMasterOneToOne(typeof(MyClass), typeof(MyOneClass)).Should().Be.True();
		}
	}
}