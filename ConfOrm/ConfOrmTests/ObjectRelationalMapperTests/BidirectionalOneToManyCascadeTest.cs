using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class BidirectionalOneToManyCascadeTest
	{
		private class Parent
		{
			public int Id { get; set; }
			public ICollection<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public Parent Owner { get; set; }
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Parent), typeof(Child) });
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Parent"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			var relation = rc.Properties.First(p => p.Name == "Children");
			relation.Should().Be.OfType<HbmBag>();
			var collection = (HbmBag)relation;
			collection.Satisfy(c => c.Inverse);
			collection.Cascade.Should().Contain("all").And.Contain("delete-orphans");
			collection.Key.Columns.First().name.Should().Be.EqualTo("Owner");
		}

		[Test]
		public void WithExplicitCascadeToAll()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();
			orm.ManyToOne<Child, Parent>();
			orm.Cascade<Parent, Child>(Cascade.All | Cascade.DeleteOrphans);
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}

		[Test]
		public void WithoutExplicitCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();
			orm.ManyToOne<Child, Parent>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}

		[Test]
		public void WithExplicitCascadeToNone()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();
			orm.ManyToOne<Child, Parent>();
			orm.Cascade<Parent, Child>(Cascade.None);
			HbmMapping mapping = GetMapping(orm);

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Parent"));
			var relation = rc.Properties.First(p => p.Name == "Children");
			var collection = (HbmBag)relation;
			collection.Cascade.Satisfy(c => string.IsNullOrEmpty(c));
		}
	}
}