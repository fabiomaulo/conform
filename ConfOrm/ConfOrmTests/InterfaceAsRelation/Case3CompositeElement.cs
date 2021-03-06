using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case3CompositeElement
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IEnumerable<IHasMessage> Components { get; set; }
			public IEnumerable<UpComponent> NestedComponents { get; set; }
		}

		private interface IHasMessage
		{
			string Message { get; set; }
		}

		private class MyComponent : IHasMessage
		{
			public string Message { get; set; }
		}

		private interface IHasSomething
		{
			string Something { get; set; }
		}

		private class UpComponent
		{
			public int SomeValue { get; set; }
			public IHasSomething NestedComponent { get; set; }
		}

		private class NestedComponent : IHasSomething
		{
			public string Something { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByComponentThenRecognizeComponentWithTheCorrectClass()
		{
			// Note: the MyComponent class must be explicitly declared in ORM or mapper.CompileMappingFor

			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			orm.AddToDomain(typeof(MyComponent));

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "Components").Single();

			hbmBagOfIRelation.ElementRelationship.Should().Be.OfType<HbmCompositeElement>()
				.And.ValueOf.Class.Should().Contain("MyComponent");
		}

		[Test]
		public void WhenInterfaceIsImplementedByNestedComponentThenRecognizeComponentWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			orm.AddToDomain(typeof(UpComponent));
			orm.AddToDomain(typeof(NestedComponent));

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmBagOfIRelation = (HbmBag)rc.Properties.Where(p => p.Name == "NestedComponents").Single();

			var upComponent = (HbmCompositeElement)hbmBagOfIRelation.ElementRelationship;
			upComponent.Properties.Where(p => p.Name == "NestedComponent").Single().Should().Be.OfType<HbmNestedCompositeElement>()
				.And.ValueOf.Class.Should().Contain("NestedComponent");
		}
	}
}