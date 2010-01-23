using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ManyToOneRelationTest
	{
		private class AEntity
		{
			public int Id { get; set; }
			public BEntity B { get; set; }
		}

		private class BEntity
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenNotRegisteredAsEntityNotRecognizeRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.ManyToOne<AEntity, BEntity>();
			mapper.IsManyToOne(typeof(AEntity), typeof(BEntity)).Should().Be.False();
		}

		[Test]
		public void WhenNotRegisteredAsEntityNotRecognizeInverseRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.ManyToOne<AEntity, BEntity>();
			mapper.IsOneToMany(typeof(BEntity), typeof(AEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToOne<AEntity, BEntity>();
			mapper.IsManyToOne(typeof (AEntity), typeof (BEntity)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeInverseRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToOne<AEntity, BEntity>();
			mapper.IsOneToMany(typeof(BEntity), typeof(AEntity)).Should().Be.True();
		}
	}
}