using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1CircularOneToMany
	{
		private interface INode
		{
			INode ParentNode { get; set; }
			IEnumerable<INode> SubNodes { get; set; }
		}

		private class Node : INode
		{
			public int Id { get; set; }
			public INode ParentNode { get; set; }
			public IEnumerable<INode> SubNodes { get; set; }
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeBidirectionalCircularOneToMany()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Node");
			var hbmBag = (HbmBag) hbmClass.Properties.Single(x => x.Name == "SubNodes");
			hbmBag.Inverse.Should().Be.True();
			hbmBag.Cascade.Should().Contain("all").And.Contain("delete-orphan");
			hbmBag.Key.ondelete.Should().Be(HbmOndelete.Noaction);
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenApplyInverse()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Node");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "SubNodes");
			hbmBag.Inverse.Should().Be.True();
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityThenApplyCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Node");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "SubNodes");
			hbmBag.Cascade.Should().Contain("all").And.Contain("delete-orphan");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnInterfaceThenApplyDeclaredCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			orm.Cascade<INode, INode>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Node");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "SubNodes");
			hbmBag.Cascade.Should().Contain("persist").And.Not.Contain("delete-orphan");
		}

		[Test]
		public void WhenInterfaceIsImplementedByEntityAndExplicitCascadeDeclaredOnConcreteThenApplyDeclaredCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			orm.Cascade<Node, Node>(CascadeOn.Persist);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Node");
			var hbmBag = (HbmBag)hbmClass.Properties.Single(x => x.Name == "SubNodes");
			hbmBag.Cascade.Should().Contain("persist").And.Not.Contain("delete-orphan");
		}
	}
}