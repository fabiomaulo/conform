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
	}
}