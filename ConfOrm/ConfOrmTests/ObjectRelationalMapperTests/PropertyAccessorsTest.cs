using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class PropertyAccessorsTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int AProp { get; set; }

			private ICollection<int> withDifferentBackField;
			public IEnumerable<int> WithDifferentBackField
			{
				get { return withDifferentBackField; }
			}

			private string readOnlyWithSameBackField;
			public string ReadOnlyWithSameBackField
			{
				get { return readOnlyWithSameBackField; }
			}

			private string sameTypeOfBackField;
			public string SameTypeOfBackField
			{
				get { return sameTypeOfBackField; }
				set { sameTypeOfBackField = value; }
			}

			public string PropertyWithoutField
			{
				get { return ""; }
			}
		}

		[Test]
		public void WhenAutoPropertyThenProperty()
		{
			var orm = new ObjectRelationalMapper();
			var member = typeof (MyClass).GetProperty("AProp");
			orm.PersistentPropertyAccessStrategy(member).Should().Be(StateAccessStrategy.Property);
		}

		[Test]
		public void WhenDifferentPropertyTypeThenField()
		{
			var orm = new ObjectRelationalMapper();
			var member = typeof(MyClass).GetProperty("WithDifferentBackField");
			orm.PersistentPropertyAccessStrategy(member).Should().Be(StateAccessStrategy.Field);
		}

		[Test]
		public void WhenNosetterPropertyWithFieldThenFieldOnSet()
		{
			var orm = new ObjectRelationalMapper();
			var member = typeof(MyClass).GetProperty("ReadOnlyWithSameBackField");
			orm.PersistentPropertyAccessStrategy(member).Should().Be(StateAccessStrategy.FieldOnSet);
		}

		[Test]
		public void WhenNosetterPropertyWithoutFieldThenReadOnly()
		{
			var orm = new ObjectRelationalMapper();
			var member = typeof(MyClass).GetProperty("PropertyWithoutField");
			orm.PersistentPropertyAccessStrategy(member).Should().Be(StateAccessStrategy.ReadOnlyProperty);
		}
	}
}