using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ManyToOneRelationInComponentTest
	{
		private class AEntity
		{
			public int Id { get; set; }
			public AComponent ValueType { get; set; }
			public string Name { get; set; }
		}

		private class AComponent
		{
			public BEntity B { get; set; }
		}

		private class BEntity
		{
			public int Id { get; set; }
		}

		[Test]
		public void CanDiscoverManyToOneFromComponentToEntity()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<AEntity>();
			orm.TablePerClass<BEntity>();
			orm.Component<AComponent>();

			orm.IsManyToOne(typeof(AComponent), typeof(BEntity)).Should().Be.True();
		}
	}
}