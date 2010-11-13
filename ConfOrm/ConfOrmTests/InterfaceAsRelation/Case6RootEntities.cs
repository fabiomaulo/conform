using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case6RootEntities
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelatedRoot RelatedRoot { get; set; }
		}

		private interface IRelatedRoot
		{
			
		}
		private class MyRelatedRoot1 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}
		private class MyRelatedRoot2 : IRelatedRoot
		{
			public int Id { get; set; }
			public IEnumerable<MyEntity> Items { get; set; }
		}

		[Test, Ignore("Not supported yet.")]
		public void WhenInterfaceIsImplementedByEntitiesThenTheColumnOfKeyShouldBeTheSameOfIdInTheAnyElement()
		{
			HbmMapping mapping = GetMapping();

			HbmClass hbmMyEntity = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmAny = (HbmAny)hbmMyEntity.Properties.Where(p => p.Name == "RelatedRoot").Single();
			var columnNameForIdInAny = hbmAny.Columns.First().name;

			HbmClass hbmMyRelatedRoot1 = mapping.RootClasses.First(r => r.Name.Contains("MyRelatedRoot1"));
			HbmClass hbmMyRelatedRoot2 = mapping.RootClasses.First(r => r.Name.Contains("MyRelatedRoot2"));
			HbmKey hbmKeyInMyRelatedRoot1 = ((HbmBag)hbmMyRelatedRoot1.Properties.Where(p => p.Name == "Items").Single()).Key;
			HbmKey hbmKeyInMyRelatedRoot2 = ((HbmBag)hbmMyRelatedRoot2.Properties.Where(p => p.Name == "Items").Single()).Key;
			hbmKeyInMyRelatedRoot1.column1.Should().Be(columnNameForIdInAny);
			hbmKeyInMyRelatedRoot2.column1.Should().Be(columnNameForIdInAny);
		}

		[Test, Ignore("Not supported yet.")]
		public void WhenInterfaceIsImplementedByEntitiesThenTheCollectionWhereShouldContainEntityName()
		{
			HbmMapping mapping = GetMapping();

			HbmClass hbmMyEntity = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			var hbmAny = (HbmAny)hbmMyEntity.Properties.Where(p => p.Name == "RelatedRoot").Single();
			var columnNameForTypeInAny = hbmAny.Columns.Skip(1).First().name;

			HbmClass hbmMyRelatedRoot1 = mapping.RootClasses.First(r => r.Name.Contains("MyRelatedRoot1"));
			HbmClass hbmMyRelatedRoot2 = mapping.RootClasses.First(r => r.Name.Contains("MyRelatedRoot2"));
			var hbmBagInMyRelatedRoot1 = (HbmBag)hbmMyRelatedRoot1.Properties.Where(p => p.Name == "Items").Single();
			var hbmBagInMyRelatedRoot2 = (HbmBag)hbmMyRelatedRoot2.Properties.Where(p => p.Name == "Items").Single();
			hbmBagInMyRelatedRoot1.Where.Should().Be(string.Format("{0} = '{1}'", columnNameForTypeInAny, typeof(MyRelatedRoot1).FullName));
			hbmBagInMyRelatedRoot2.Where.Should().Be(string.Format("{0} = '{1}'", columnNameForTypeInAny, typeof(MyRelatedRoot2).FullName));
		}

		private HbmMapping GetMapping()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelatedRoot1>();
			orm.TablePerClass<MyRelatedRoot2>();

			var mapper = new Mapper(orm);
			return mapper.CompileMappingFor(new[] {typeof (MyEntity), typeof (MyRelatedRoot1), typeof (MyRelatedRoot2)});
		}
	}
}