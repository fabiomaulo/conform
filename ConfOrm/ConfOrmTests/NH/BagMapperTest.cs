using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class BagMapperTest
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
		public void SetInverse()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Inverse(true);
			hbm.Inverse.Should().Be.True();
			mapper.Inverse(false);
			hbm.Inverse.Should().Be.False();
		}

		[Test]
		public void SetMutable()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Mutable(true);
			hbm.Mutable.Should().Be.True();
			mapper.Mutable(false);
			hbm.Mutable.Should().Be.False();
		}

		[Test]
		public void SetWhere()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Where("c > 10");
			hbm.Where.Should().Be.EqualTo("c > 10");
		}

		[Test]
		public void SetBatchSize()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.BatchSize(10);
			hbm.BatchSize.Should().Be.EqualTo(10);
		}

		[Test]
		public void SetLazy()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
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
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			bool kmCalled = false;
			mapper.Key(km => kmCalled = true);
			hbm.Key.Should().Not.Be.Null();
			kmCalled.Should().Be.True();
		}

		[Test]
		public void SetCollectionTypeByWrongTypeShouldThrow()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			ActionAssert.Throws<ArgumentNullException>(() => mapper.Type(null));
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Type(typeof(object)));
		}

		[Test]
		public void SetCollectionTypeByGenericType()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type<FakeUserCollectionType>();
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void SetCollectionTypeByType()
		{
			var hbm = new HbmBag();
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type(typeof(FakeUserCollectionType));
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void CanChangeAccessor()
		{
			var hbm = new HbmBag {name = "Children"};
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Access(Accessor.Field);

			hbm.Access.Should().Not.Be.Null();
		}

		[Test]
		public void CanSetCache()
		{
			var hbm = new HbmBag { name = "Children" };
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(x=> x.Region("pizza"));

			hbm.cache.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var hbm = new HbmBag { name = "Children" };
			var mapper = new BagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(ch => ch.Region("pizza"));
			mapper.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = hbm.cache;
			hbmCache.Should().Not.Be.Null();
			hbmCache.region.Should().Be("pizza");
			hbmCache.usage.Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}
	}
}