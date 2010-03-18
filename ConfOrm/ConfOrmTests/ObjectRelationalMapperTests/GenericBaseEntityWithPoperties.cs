using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class GenericBaseEntityWithPoperties
	{
		private class MyEntity
		{
			public int Id { get; set; }
		}
		private class MyClass<T>
		{
			public int Id { get; set; }
			public ICollection<T> Collection { get; set; }
			public ICollection<T> PrivateCollection { get; private set; }
		}

		private class Inerited : MyClass<MyEntity>
		{
		}

		[Test]
		public void WhenBasePublicPropertiesThenIsPersistent()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Inerited>();
			PropertyInfo collectionProp = typeof (Inerited).GetProperty("Collection",
			                                                            BindingFlags.Public | BindingFlags.Instance
			                                                            | BindingFlags.FlattenHierarchy);
			orm.IsPersistentProperty(collectionProp).Should().Be.True();
		}

		[Test]
		public void WhenBasePrivateSetPropertiesThenIsPersistent()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Inerited>();
			PropertyInfo collectionProp = typeof(Inerited).GetProperty("PrivateCollection",
																																	BindingFlags.Public | BindingFlags.Instance
																																	| BindingFlags.FlattenHierarchy);
			orm.IsPersistentProperty(collectionProp).Should().Be.True();
		}
	}
}