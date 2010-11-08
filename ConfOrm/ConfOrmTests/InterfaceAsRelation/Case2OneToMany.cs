using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case2OneToMany
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IEnumerable<IRelation> Relations { get; set; }
		}

		private interface IRelation
		{
		}

		private class MyRelationL0
		{
			public int Id { get; set; }
		}

		private class MyRelationL1 : MyRelationL0, IRelation
		{
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeManyToOneWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelationL0>();

			// Note : the class MyRelationL1 should be declared as part of the domain in some way
			// I can "emulate" what will happen in general using ConfORM adding MyRelationL1 as a class where the mapping is needed

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(MyRelationL1) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Relations").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.OfType<HbmOneToMany>()
				.And.ValueOf.Class.Should().Contain("MyRelationL1");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndDomainClassIsAddedToOrmThenRecognizeManyToOneWithTheCorrectClass()
		{
			// Note : in this case the mapping for MyRelationL1 is not required and it is never used explicitly.
			// I have to "present" the class to the DomainInspector as part of my domain

			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelationL0>();

			orm.AddToDomain(typeof(MyRelationL1));

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Relations").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.OfType<HbmOneToMany>()
				.And.ValueOf.Class.Should().Contain("MyRelationL1");
		}
	}
}