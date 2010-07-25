using System;
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
			public IRelation TheOtherRelation { get; set; }
		}

		private interface IRelation
		{
			
		}

		private class Relation: IRelation
		{

		}

		private class Whatever
		{
			
		}

		[Test]
		public void AssignCascadeStyle()
		{
			var hbmMapping = new HbmMapping();
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm, hbmMapping);
			mapper.Cascade(Cascade.Persist | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbmMapping = new HbmMapping();
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm, hbmMapping);
			mapper.Cascade(Cascade.Persist | Cascade.DeleteOrphans | Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w => w.Satisfy(cascade => !cascade.Contains("orphan")));
		}

		[Test]
		public void CanSetAccessor()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);
			mapper.Column(cm => cm.Name("RelationId"));

			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.Single().name.Should().Be("RelationId");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.Name("Relation"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			ActionAssert.Throws<ConfOrm.MappingException>(() => mapper.Column(cm => cm.Length(50)));
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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

		[Test]
		public void WhenSetFetchModeToJoinThenSetFetch()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Fetch(FetchMode.Join);

			mapping.fetch.Should().Be(HbmFetchMode.Join);
			mapping.fetchSpecified.Should().Be.True();
		}

		[Test]
		public void WhenSetFetchModeToSelectThenResetFetch()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Fetch(FetchMode.Select);

			mapping.fetch.Should().Be(HbmFetchMode.Select);
			mapping.fetchSpecified.Should().Be.False();
		}

		[Test]
		public void CanForceClassRelation()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Class(typeof (Relation));

			mapping.Class.Should().Contain("Relation").And.Not.Contain("IRelation");
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof (MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Class(typeof (Whatever)));
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Lazy(LazyRelation.NoProxy);
			mapping.Lazy.Should().Have.Value();
			mapping.Lazy.Should().Be(HbmLaziness.NoProxy);
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Update(false);
			hbm.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Insert(false);
			hbm.insert.Should().Be.False();
		}
	}
}