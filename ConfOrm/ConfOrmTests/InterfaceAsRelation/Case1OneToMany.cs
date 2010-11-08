using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1OneToMany
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IEnumerable<IRelation> Relations { get; set; }
			public IEnumerable<Relation1> Relations1 { get; set; }
		}

		private interface IRelation
		{
		}

		private class MyRelation : IRelation
		{
			public int Id { get; set; }
		}

		private class Relation1
		{
		}

		private class MyRelation1 : Relation1
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeOneToMany()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyRelation1>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Relations").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.InstanceOf<HbmOneToMany>();
		}

		[Test, Ignore("Not supported yet.")]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeOneTManyWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyRelation1>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag) rc.Properties.Where(p => p.Name == "Relations").Single();
			
			hbmBagOfIRelation.ElementRelationship.Should().Be.OfType<HbmOneToMany>()
				.And.ValueOf.Class.Should().Contain("MyRelation");
		}

		[Test, Ignore("Not supported yet.")]
		public void WhenClassIsImplementedByEntityThenRecognizeOneToManyWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyRelation1>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Relations1").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.OfType<HbmOneToMany>()
				.And.ValueOf.Class.Should().Contain("MyRelation1");
		}

		[Test]
		public void WhenClassIsImplementedByEntityThenRecognizeOneToMany()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyRelation1>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Relations1").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.InstanceOf<HbmOneToMany>();
		}
	}
}