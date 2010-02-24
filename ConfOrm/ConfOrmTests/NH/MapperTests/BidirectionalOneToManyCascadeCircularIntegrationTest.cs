using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class BidirectionalOneToManyCascadeCircularIntegrationTest
	{
		private MemberInfo parentProperty = typeof(Node).GetProperty("Parent");
		private MemberInfo subnodesProperty = typeof(Node).GetProperty("Subnodes");

		private class Node
		{
			public int Id { get; set; }
			public Node Parent { get; set; }
			public IEnumerable<Node> Subnodes { get; set; }
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Node)});
		}

		[Test]
		public void ApplyCascadeOnChildNotInParentByDefault()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			HbmMapping mapping = GetMapping(orm);

			HbmClass rc = mapping.RootClasses.Single();
			var parent = (HbmManyToOne)rc.Properties.Single(p => p.Name == "Parent");
			parent.cascade.Should().Be.Null();
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "Subnodes");
			subNodes.cascade.Should().Contain("all").And.Contain("delete-orphan");
		}

		[Test]
		public void WhenExplicitRequiredByClassApplyCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			orm.Cascade<Node, Node>(Cascade.Persist);

			HbmMapping mapping = GetMapping(orm);

			HbmClass rc = mapping.RootClasses.Single();
			var parent = (HbmManyToOne)rc.Properties.Single(p => p.Name == "Parent");
			parent.cascade.Should().Contain("persist");
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "Subnodes");
			subNodes.cascade.Should().Contain("persist");
		}

		[Test]
		public void WhenExplicitRequiredApplyCascadeOnParent()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var mapper = new Mapper(orm);
			mapper.Customize<Node>(node=> node.ManyToOne(n=>n.Parent, m=> m.Cascade(Cascade.Persist)));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			HbmClass rc = mapping.RootClasses.Single();
			var parent = (HbmManyToOne)rc.Properties.Single(p => p.Name == "Parent");
			parent.cascade.Should().Contain("persist");
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "Subnodes");
			subNodes.cascade.Should().Contain("all").And.Contain("delete-orphan");
		}

		[Test]
		public void WhenExplicitRequiredThroughClassCustomizerApplyCascadeOnParent()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();

			var mapper = new Mapper(orm);
			mapper.Class<Node>(node => node.ManyToOne(n => n.Parent, m => m.Cascade(Cascade.Persist)));

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Node) });

			HbmClass rc = mapping.RootClasses.Single();
			var parent = (HbmManyToOne)rc.Properties.Single(p => p.Name == "Parent");
			parent.cascade.Should().Contain("persist");
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "Subnodes");
			subNodes.cascade.Should().Contain("all").And.Contain("delete-orphan");
		}
	}
}