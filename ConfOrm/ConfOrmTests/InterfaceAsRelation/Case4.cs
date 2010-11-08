using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case4
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRootRelation Relation { get; set; }
			public IDeepRelation DeepRelation { get; set; }
		}

		private interface IRootRelation
		{
		}

		private interface IDeepRelation
		{
		}

		private class MyRelation : IRootRelation
		{
			public int Id { get; set; }
		}

		private class MyDeepRelation : MyRelation, IDeepRelation
		{
		}

		private class MyOtherEntity : IRootRelation
		{
			public int Id { get; set; }
		}

		private class MyOtherDeepRelation : MyOtherEntity, IDeepRelation
		{
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntitiesThenDoesNotRecognizeAsManyToOne()
		{
			// when there are more then one Hierarchy tree then it can't be directly interpreted as ManyToOne
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyOtherEntity>();

			var mapper = new Mapper(orm);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity), typeof(MyRelation), typeof(MyDeepRelation), typeof(MyOtherEntity), typeof(MyOtherDeepRelation) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			IEntityPropertyMapping propertyRelation = rc.Properties.Where(p => p.Name == "Relation").Single();
			propertyRelation.Should().Not.Be.InstanceOf<HbmManyToOne>();

			IEntityPropertyMapping propertyDeepRelation = rc.Properties.Where(p => p.Name == "DeepRelation").Single();
			propertyDeepRelation.Should().Not.Be.InstanceOf<HbmManyToOne>();
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntitiesThenRecognizeHeterogeneousAssociation()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();
			orm.TablePerClass<MyOtherEntity>();

			var mapper = new Mapper(orm);
			HbmMapping mapping = mapper.CompileMappingFor(new[] {typeof (MyEntity), typeof (MyRelation), typeof (MyDeepRelation), typeof (MyOtherEntity), typeof (MyOtherDeepRelation)});

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			IEntityPropertyMapping propertyRelation = rc.Properties.Where(p => p.Name == "Relation").Single();
			propertyRelation.Should().Be.InstanceOf<HbmAny>();

			IEntityPropertyMapping propertyDeepRelation = rc.Properties.Where(p => p.Name == "DeepRelation").Single();
			propertyDeepRelation.Should().Be.InstanceOf<HbmAny>();
		}
	}
}