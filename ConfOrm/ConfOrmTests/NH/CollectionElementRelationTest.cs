using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class CollectionElementRelationTest
	{
		private class Address
		{
			
		}
		[Test]
		public void AssignElementRelationship()
		{
			object relationField = null;
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => relationField = element);
			hbm.Element(x => { });
			relationField.Should().Not.Be.Null().And.Be.OfType<HbmElement>().And.ValueOf.Type.Satisfy(
				a => !string.IsNullOrEmpty(a.name));
		}

		[Test]
		public void AssignOneToManyRelationship()
		{
			object relationField = null;
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => relationField = element);
			hbm.OneToMany(x => { });
			relationField.Should().Not.Be.Null().And.Be.OfType<HbmOneToMany>().And.ValueOf.@class.Satisfy(
				a => !string.IsNullOrEmpty(a));
		}

		[Test]
		public void AssignManyToManyRelationship()
		{
			object relationField = null;
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => relationField = element);
			hbm.ManyToMany(x => { });
			relationField.Should().Not.Be.Null().And.Be.OfType<HbmManyToMany>().And.ValueOf.@class.Satisfy(
				a => !string.IsNullOrEmpty(a));
		}

		[Test]
		public void AssignComponentRelationship()
		{
			object relationField = null;
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => relationField = element);
			hbm.Component(comp => { });
			relationField.Should().Not.Be.Null().And.Be.OfType<HbmCompositeElement>().And.ValueOf.@class.Satisfy(
				a => !string.IsNullOrEmpty(a));
		}

		[Test]
		public void WhenMapElementRelationshipThenCallAction()
		{
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => { });
			bool called = false;
			hbm.Element(x=> called = true);
			called.Should().Be.True();
		}

		[Test]
		public void WhenMapOneToManyRelationshipThenCallAction()
		{
			var hbm = new CollectionElementRelation(typeof(Address), new HbmMapping(), element => { });
			bool called = false;
			hbm.OneToMany(x => called = true);
			called.Should().Be.True();
		}
	}
}