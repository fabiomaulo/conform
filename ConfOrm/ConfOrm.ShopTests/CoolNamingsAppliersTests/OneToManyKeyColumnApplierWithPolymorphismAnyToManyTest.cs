using System.Collections.Generic;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.CoolNamingsAppliersTests
{
	public class OneToManyKeyColumnApplierWithPolymorphismAnyToManyTest
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelatedRoot RelatedRoot { get; set; }
		}

		private interface IRelatedRoot
		{

		}
		private class MyRelatedRoot1 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}
		private class MyRelatedRoot2 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}

		[Test]
		public void WhenAnyToManyThenNoMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelatedRoot1>();
			orm.TablePerClass<MyRelatedRoot2>();

			var applier = new OneToManyKeyColumnApplier(orm);

			applier.Match(new PropertyPath(null, ForClass<MyRelatedRoot1>.Property(x => x.Items))).Should().Be.False();
			applier.Match(new PropertyPath(null, ForClass<MyRelatedRoot2>.Property(x => x.Items))).Should().Be.False();
		}
	}
}