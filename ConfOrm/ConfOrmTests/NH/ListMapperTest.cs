using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;
using ListMapper = ConfOrm.NH.ListMapper;

namespace ConfOrmTests.NH
{
	public class ListMapperTest
	{
		private class Animal
		{
			public int Id { get; set; }
			private ICollection<Animal> children;
			public ICollection<Animal> Children
			{
				get { return children; }
			}
		}

		[Test]
		public void SetListIndex()
		{
			var hbm = new HbmList();
			new ListMapper(typeof(Animal), typeof(Animal), hbm);
			hbm.Item.Should().Not.Be.Null().And.Be.OfType<HbmListIndex>();
		}

		[Test]
		public void SetInverse()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Inverse(true);
			hbm.Inverse.Should().Be.True();
			mapper.Inverse(false);
			hbm.Inverse.Should().Be.False();
		}

		[Test]
		public void SetMutable()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Mutable(true);
			hbm.Mutable.Should().Be.True();
			mapper.Mutable(false);
			hbm.Mutable.Should().Be.False();
		}

		[Test]
		public void SetWhere()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Where("c > 10");
			hbm.Where.Should().Be.EqualTo("c > 10");
		}

		[Test]
		public void SetBatchSize()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.BatchSize(10);
			hbm.BatchSize.Should().Be.EqualTo(10);
		}

		[Test]
		public void SetLazy()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Lazy(CollectionLazy.Extra);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.Extra);
			mapper.Lazy(CollectionLazy.NoLazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.False);
			mapper.Lazy(CollectionLazy.Lazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.True);
		}

		[Test]
		public void CallKeyMapper()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			bool kmCalled = false;
			mapper.Key(km => kmCalled = true);
			hbm.Key.Should().Not.Be.Null();
			kmCalled.Should().Be.True();
		}
		[Test]
		public void SetCollectionTypeByGenericType()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type<FakeUserCollectionType>();
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void SetCollectionTypeByType()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type(typeof(FakeUserCollectionType));
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void CanChangeAccessor()
		{
			var hbm = new HbmList { name = "Children" };
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Access(Accessor.Field);

			hbm.Access.Should().Not.Be.Null();
		}

		[Test]
		public void CanChangeListIndexDef()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Index(i => { i.Column("Pepe");
			                  	i.Base(1); });
			hbm.Item.Should().Be.OfType<HbmListIndex>().And.Value.Satisfy(i => i.column1 == "Pepe" && i.@base == "1");
		}

		[Test]
		public void CanSetCache()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(x => x.Region("pizza"));

			hbm.cache.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(ch => ch.Region("pizza"));
			mapper.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = hbm.cache;
			hbmCache.Should().Not.Be.Null();
			hbmCache.region.Should().Be("pizza");
			hbmCache.usage.Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}

		[Test]
		public void CanSetAFilterThroughAction()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == "condition1");
		}

		[Test]
		public void CanSetMoreFiltersThroughAction()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "condition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenSameNameThenOverrideCondition()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			mapper.Filter("filter1", f => f.Condition("anothercondition1"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "anothercondition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenActionIsNullThenAddFilterName()
		{
			var hbm = new HbmList();
			var mapper = new ListMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", null);
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == null);
		}
	}
}