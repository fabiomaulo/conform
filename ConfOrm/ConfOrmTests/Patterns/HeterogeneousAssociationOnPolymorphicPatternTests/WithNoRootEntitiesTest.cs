using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.HeterogeneousAssociationOnPolymorphicPatternTests
{
	public class WithNoRootEntitiesTest
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelated Related { get; set; }
		}

		private interface IRelated
		{

		}

		private class MyRoot1
		{
			public int Id { get; set; }
		}

		private class MyRelatedNoRoot1 : MyRoot1, IRelated
		{
		}

		private class MyRoot2
		{
			public int Id { get; set; }
		}

		private class MyRelatedNoRoot2 : MyRoot2, IRelated
		{
		}

		[Test]
		public void WhenRelationIsWithNoRootEntitiesThenMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRoot1>();
			orm.TablePerClass<MyRoot2>();

			// in this test I'm adding the two classes through the ObjectRelationalMapper but in a normal usage
			// this work will be done by the mapper when I ask for the mapping of MyRelatedNoRoot1 and MyRelatedNoRoot2
			orm.AddToDomain(typeof(MyRelatedNoRoot1));
			orm.AddToDomain(typeof(MyRelatedNoRoot2));

			var pattern = new HeterogeneousAssociationOnPolymorphicPattern(orm);
			pattern.Match(ForClass<MyEntity>.Property(x => x.Related)).Should().Be.True();
		}

	}
}