using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ReadOnlyProperties
	{
		public class MyEntity
		{
			public int Id { get; set; }
			public string ReadOnly
			{
				get { return ""; }
			}

			public int NoReadOnly { get; set; }
		}

		[Test]
		public void WhenReadOnlyPropThenNoPersistent()
		{
			var orm = new ObjectRelationalMapper();
			orm.IsPersistentProperty(typeof (MyEntity).GetProperty("ReadOnly")).Should().Be.False();
		}

		[Test]
		public void WhenExplicitDeclaredPersistentThenPersistent()
		{
			var orm = new ObjectRelationalMapper();
			orm.PersistentProperty<MyEntity>(me => me.ReadOnly);
			orm.IsPersistentProperty(typeof(MyEntity).GetProperty("ReadOnly")).Should().Be.True();
		}
	}
}