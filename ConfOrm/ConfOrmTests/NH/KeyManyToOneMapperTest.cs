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
	public class KeyManyToOneMapperTest
	{
		private class MyComponent
		{
			public Relation Relation { get; set; }
			public IRelation TheOtherRelation { get; set; }
		}

		private interface IRelation
		{

		}

		private class Relation : IRelation
		{

		}

		private class Whatever
		{

		}

		[Test]
		public void WhenAssignCascadeStyleThenNotThrows()
		{
			var hbmMapping = new HbmMapping();
			var hbm = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(null, hbm, hbmMapping);
			mapper.Executing(m => m.Cascade(CascadeOn.Persist | CascadeOn.Remove)).NotThrows();
		}

		[Test]
		public void CanSetAccessor()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var hbm = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, hbm, hbmMapping);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var hbm = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, hbm, hbmMapping);
			mapper.Column(cm => cm.Name("RelationId"));

			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.Single().name.Should().Be("RelationId");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.Name("Relation"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm =>
			{
				cm.UniqueKey("theUnique");
				cm.NotNullable(true);
			});
			mapping.column.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetColumnValuesMoreThanOnesThenMergeColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.UniqueKey("theUnique"));
			mapper.Column(cm => cm.NotNullable(true));

			var hbmColumn = mapping.Columns.Single();
			hbmColumn.uniquekey.Should().Be("theUnique");
			hbmColumn.notnull.Should().Be(true);
			hbmColumn.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
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
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapper.Executing(m=> m.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}

		[Test]
		public void WhenSetFetchModeToJoinThenNotThrows()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);

			mapper.Executing(m=> m.Fetch(FetchMode.Join)).NotThrows();
		}

		[Test]
		public void CanForceClassRelation()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("TheOtherRelation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);

			mapper.Class(typeof(Relation));

			mapping.Class.Should().Contain("Relation").And.Not.Contain("IRelation");
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("TheOtherRelation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);

			mapper.Executing(m=> m.Class(typeof(Whatever))).Throws<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("TheOtherRelation");
			var mapping = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, mapping, hbmMapping);
			
			mapper.Lazy(LazyRelation.NoProxy);
			mapping.Lazy.Should().Have.Value();
			mapping.Lazy.Should().Be(HbmRestrictedLaziness.False);

			mapper.Lazy(LazyRelation.NoLazy);
			mapping.Lazy.Should().Have.Value();
			mapping.Lazy.Should().Be(HbmRestrictedLaziness.False);

			mapper.Lazy(LazyRelation.Proxy);
			mapping.Lazy.Should().Have.Value();
			mapping.Lazy.Should().Be(HbmRestrictedLaziness.Proxy);
		}

		[Test]
		public void CanSetFk()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyComponent).GetProperty("Relation");
			var hbm = new HbmKeyManyToOne();
			var mapper = new KeyManyToOneMapper(member, hbm, hbmMapping);

			mapper.ForeignKey("MyFkName");

			hbm.foreignkey.Should().Be("MyFkName");
		}
	}
}