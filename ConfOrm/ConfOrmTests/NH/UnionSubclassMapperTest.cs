using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class UnionSubclassMapperTest
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class Inherited : EntitySimple
		{
		}

		private class Z
		{

		}
		[Test]
		public void WhenMapDocHasDefaultUnionSubclassElementHasClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new UnionSubclassMapper(subClass, mapdoc);
			mapdoc.UnionSubclasses[0].Name.Should().Be.EqualTo(typeof(Inherited).Name);
		}

		[Test]
		public void WhenMapDocHasDefaultUnionSubclassElementHasExtendsClassName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping { assembly = subClass.Assembly.FullName, @namespace = subClass.Namespace };
			new UnionSubclassMapper(subClass, mapdoc);
			mapdoc.UnionSubclasses[0].extends.Should().Be.EqualTo(typeof(EntitySimple).Name);
		}

		[Test]
		public void SetEntityName()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.EntityName("pepe");

			var hbmEntity = mapdoc.UnionSubclasses[0];
			hbmEntity.EntityName.Should().Be("pepe");
		}

		[Test]
		public void SetProxy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.Proxy(subClass);

			var hbmEntity = mapdoc.UnionSubclasses[0];
			hbmEntity.Proxy.Should().Contain("Inherited");
		}

		[Test]
		public void SetWrongProxy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			ActionAssert.Throws<MappingException>(() => mapper.Proxy(typeof(Z)));
		}

		[Test]
		public void SetLazy()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.Lazy(true);

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.UseLazy.Should().Not.Have.Value();

			mapper.Lazy(false);
			hbmEntity.UseLazy.Should().Be(false);
		}

		[Test]
		public void SetDynamicUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.DynamicUpdate(true);

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.DynamicUpdate.Should().Be(true);
		}

		[Test]
		public void SetDynamicInsert()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.DynamicInsert(true);

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.DynamicInsert.Should().Be(true);
		}

		[Test]
		public void SetBatchSize()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.BatchSize(10);

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.BatchSize.Should().Be(10);
		}

		[Test]
		public void SetSelectBeforeUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.SelectBeforeUpdate(true);

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.SelectBeforeUpdate.Should().Be(true);
		}

		[Test]
		public void SetLoader()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.Loader("blah");

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.SqlLoader.Should().Not.Be.Null();
			hbmEntity.SqlLoader.queryref.Should().Be("blah");
		}

		[Test]
		public void SetSqlInsert()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.SqlInsert("blah");

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.SqlInsert.Should().Not.Be.Null();
			hbmEntity.SqlInsert.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlUpdate()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.SqlUpdate("blah");

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.SqlUpdate.Should().Not.Be.Null();
			hbmEntity.SqlUpdate.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlDelete()
		{
			var subClass = typeof(Inherited);
			var mapdoc = new HbmMapping();
			var mapper = new UnionSubclassMapper(subClass, mapdoc);
			mapper.SqlDelete("blah");

			var hbmEntity = mapdoc.UnionSubclasses[0];

			hbmEntity.SqlDelete.Should().Not.Be.Null();
			hbmEntity.SqlDelete.Text[0].Should().Be("blah");
		}

	}
}