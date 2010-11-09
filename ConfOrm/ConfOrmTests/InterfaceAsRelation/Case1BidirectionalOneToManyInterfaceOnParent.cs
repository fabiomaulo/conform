using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1BidirectionalOneToManyInterfaceOnParent
	{
		private interface IParent
		{
			IEnumerable<Child> Children { get; set; }
		}

		private class Parent: IParent
		{
			public int Id { get; set; }
			public IEnumerable<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public IParent Parent { get; set; }
		}

		[Test, Ignore("Not supported yet.")]
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
	}
}