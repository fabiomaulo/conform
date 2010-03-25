using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class AnyMapperTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }			
		}

		[Test]
		public void AtCreationSetIdType()
		{
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof (int), hbmAny);
			hbmAny.idtype.Should().Be("Int32");
		}

		[Test]
		public void AtCreationSetTheTwoRequiredColumnsNodes()
		{
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny);
			hbmAny.Columns.Should().Have.Count.EqualTo(2);
			hbmAny.Columns.Select(c => c.name).All(n => n.Satisfy(name=> !string.IsNullOrEmpty(name)));
		}

		[Test]
		public void CanSetIdTypeThroughIType()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.IdType(NHibernateUtil.Int64);
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughGenericMethod()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.IdType<long>();
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughType()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.IdType(typeof(long));
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetMetaTypeThroughIType()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.MetaType(NHibernateUtil.Character);
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughGenericMethod()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.MetaType<char>();
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughType()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.MetaType(typeof(char));
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetCascade()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.Cascade(Cascade.All);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void AutoCleanInvalidCascade()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.Cascade(Cascade.All | Cascade.DeleteOrphans);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void CanSetIndex()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.Index("pizza");
			hbmAny.index.Should().Be("pizza");
		}

		[Test]
		public void CanSetLazy()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			mapper.Lazy(true);
			hbmAny.lazy.Should().Be(true);
		}

		[Test]
		public void WhenSetIdColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => idcm.Length(10), metacm => { });
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[0].Should().Be.SameInstanceAs(columnsAfter[0]);
			columnsBefore[0].length.Should().Be("10");
		}

		[Test]
		public void WhenSetMetaColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => { }, metacm => metacm.Length(500));
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[1].Should().Be.SameInstanceAs(columnsAfter[1]);
			columnsBefore[1].length.Should().Be("500");
		}
	}
}