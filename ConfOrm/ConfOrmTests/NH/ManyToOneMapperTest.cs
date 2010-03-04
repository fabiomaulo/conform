using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ManyToOneMapperTest
	{
		private class MyClass
		{
			public Relation Relation { get; set; }
		}

		private class Relation
		{
			
		}

		[Test]
		public void AssignCascadeStyle()
		{
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm);
			mapper.Cascade(Cascade.Persist | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm);
			mapper.Cascade(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w => w.Satisfy(cascade => !cascade.Contains("orphan")));
		}

		[Test]
		public void CanSetAccessor()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm);
			mapper.Column(cm => cm.Name("RelationId"));

			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.Single().name.Should().Be("RelationId");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Column(cm => cm.Name("Relation"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.UniqueKey("theUnique");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Be.Null();
			mapping.uniquekey.Should().Be("theUnique");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("BIGINT");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnesThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Column(cm => cm.UniqueKey("theUnique"));
			mapper.Column(cm => cm.NotNullable(true));

			mapping.Items.Should().Be.Null();
			mapping.uniquekey.Should().Be("theUnique");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Columns(cm =>
			{
				cm.Name("column1");
				cm.Length(50);
			}, cm =>
			{
				cm.Name("column2");
				cm.SqlType("VARCHAR(10)");
			});
			mapping.Columns.Should().Have.Count.EqualTo(2);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAutoassignColumnNames()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			ActionAssert.Throws<ConfOrm.MappingException>(() => mapper.Column(cm => cm.Length(50)));
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping);
			mapper.Column("pizza");
			mapper.NotNullable(true);
			mapper.Unique(true);
			mapper.UniqueKey("AA");
			mapper.Index("II");

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
			mapping.notnull.Should().Be(true);
			mapping.unique.Should().Be(true);
			mapping.uniquekey.Should().Be("AA");
			mapping.index.Should().Be("II");
		}

	}
}