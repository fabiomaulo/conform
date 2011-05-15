using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1BidirectionalOneToManyInterfaceOnChild
	{
		private interface IChild
		{
			Parent Parent { get; set; }
		}

		private class Parent
		{
			public int Id { get; set; }
			public IEnumerable<IChild> Children { get; set; }
		}

		private class Child : IChild
		{
			public int Id { get; set; }
			public Parent Parent { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeBidirectionalOneToMany()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Inverse.Should().Be.True();
			hbmBag.Cascade.Should().Contain("all").And.Contain("delete-orphan");
			hbmBag.Key.ondelete.Should().Be(HbmOndelete.Cascade);
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenApplyInverse()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Inverse.Should().Be.True();
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenApplyCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Cascade.Should().Contain("all").And.Contain("delete-orphan");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnInterfaceThenApplyDeclaredCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			orm.Cascade<Parent, IChild>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Cascade.Should().Contain("persist").And.Not.Contain("delete-orphan");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnConcreteThenApplyDeclaredCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			orm.Cascade<Parent, Child>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Cascade.Should().Contain("persist").And.Not.Contain("delete-orphan");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnInterfaceThenNotApplyOnDelete()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			orm.Cascade<Parent, IChild>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Key.ondelete.Should().Be(HbmOndelete.Noaction);
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnConcreteThenNotApplyOnDelete()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();

			orm.Cascade<Parent, Child>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Parent) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Parent");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "Children");
			hbmBag.Key.ondelete.Should().Be(HbmOndelete.Noaction);
		}
	}
}