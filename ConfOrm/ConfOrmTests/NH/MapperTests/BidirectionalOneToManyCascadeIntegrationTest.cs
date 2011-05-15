using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class BidirectionalOneToManyCascadeIntegrationTest
	{
		private class UpAggregateRoot
		{
			public int Id { get; set; }
			public ICollection<DownAggregateRoot> DownAggregateRoots { get; set; }
		}

		private class DownAggregateRoot
		{
			public int Id { get; set; }
			public UpAggregateRoot UpAggregateRoot { get; set; }
		}

		[Test]
		public void WhenCascadeIsTurnedOffInOrmShouldntApplyCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(new[] { typeof(UpAggregateRoot), typeof(DownAggregateRoot) });
			orm.Cascade<UpAggregateRoot, DownAggregateRoot>(CascadeOn.None);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(UpAggregateRoot) });

			HbmClass rc = mapping.RootClasses.Single();
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "DownAggregateRoots");
			subNodes.cascade.Should().Be.Null();
		}

		[Test]
		public void WhenCascadeIsTurnedOffInOrmShouldntApplyOnDeleteCascade()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(new[] { typeof(UpAggregateRoot), typeof(DownAggregateRoot) });
			orm.Cascade<UpAggregateRoot, DownAggregateRoot>(CascadeOn.None);

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(UpAggregateRoot) });

			HbmClass rc = mapping.RootClasses.Single();
			var subNodes = (HbmBag)rc.Properties.Single(p => p.Name == "DownAggregateRoots");
			subNodes.Key.ondelete.Should().Be(HbmOndelete.Noaction);
		}
	}
}