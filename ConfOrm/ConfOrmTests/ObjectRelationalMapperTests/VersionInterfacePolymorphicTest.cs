using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class VersionInterfacePolymorphicTest
	{
		private interface IVersionedEntity
		{
			int Version { get; set; }
		}

		private class MyVersionedEntityOnInterface : IVersionedEntity
		{
			public string Name { get; set; }
			public int Version { get; set; }
		}

		[Test]
		public void WhenVersionPropertyDefinedOnInterfaceThenRecognizeItOnConcreteClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyVersionedEntityOnInterface>();
			orm.VersionProperty<IVersionedEntity>(versionedEntity => versionedEntity.Version);

			orm.IsVersion(ForClass<MyVersionedEntityOnInterface>.Property(e => e.Version)).Should().Be.True();
		}
	}
}