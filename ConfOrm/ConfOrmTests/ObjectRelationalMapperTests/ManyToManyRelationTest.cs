using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ManyToManyRelationTest
	{
		private class AEntity
		{
			public int Id { get; set; }
		}

		private class BEntity
		{
			public int Id { get; set; }
		}

		// Commented because a ManyToMany can be between an entity and an interface not explicit mapped as entity
		//[Test]
		//public void WhenNotRegisteredAsEntityNotRecognizeRelation()
		//{
		//  var mapper = new ObjectRelationalMapper();
		//  mapper.ManyToMany<AEntity, BEntity>();
		//  mapper.IsManyToMany(typeof(AEntity), typeof(BEntity)).Should().Be.False();
		//}

		// Commented because a ManyToMany can be between an entity and an interface not explicit mapped as entity
		//[Test]
		//public void WhenNotRegisteredAsEntityNotRecognizeInverseRelation()
		//{
		//  var mapper = new ObjectRelationalMapper();
		//  mapper.ManyToMany<AEntity, BEntity>();
		//  mapper.IsManyToMany(typeof(BEntity), typeof(AEntity)).Should().Be.False();
		//}

		[Test]
		public void WhenExplicitRegisteredRecognizeRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToMany<AEntity, BEntity>();
			mapper.IsManyToMany(typeof(AEntity), typeof(BEntity)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeInverseRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToMany<AEntity, BEntity>();
			mapper.IsManyToMany(typeof(BEntity), typeof(AEntity)).Should().Be.True();
		}

		[Test]
		public void WhenManyToManyIsNotManyToOne()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToMany<AEntity, BEntity>();
			mapper.IsOneToMany(typeof(AEntity), typeof(BEntity)).Should().Be.False();
			mapper.IsManyToOne(typeof(AEntity), typeof(BEntity)).Should().Be.False();
			mapper.IsOneToMany(typeof(BEntity), typeof(AEntity)).Should().Be.False();
			mapper.IsManyToOne(typeof(BEntity), typeof(AEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeMaster()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToMany<AEntity, BEntity>();
			mapper.IsMasterManyToMany(typeof(AEntity), typeof(BEntity)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeInverseRelationAsImplicit()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AEntity>();
			mapper.TablePerClass<BEntity>();
			mapper.ManyToMany<AEntity, BEntity>();
			mapper.IsMasterManyToMany(typeof(BEntity), typeof(AEntity)).Should().Be.False();
		}
	}
}