using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class MapKeyMapperTest
	{
		[Test]
		public void CatSetColumnByName()
		{
			var hbm = new HbmMapKey();
			var mapper = new MapKeyMapper(hbm);
			mapper.Column("pizza");
			hbm.column.Should().Be("pizza");
		}

		[Test]
		public void CatSetLength()
		{
			var hbm = new HbmMapKey();
			var mapper = new MapKeyMapper(hbm);
			mapper.Length(55);
			hbm.length.Should().Be("55");
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Column(cm =>
			{
				cm.Length(50);
			});
			mapping.Items.Should().Be.Null();
			mapping.length.Should().Be("50");
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Type<MyType>();
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
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapper.Executing(m => m.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var mapping = new HbmMapKey();
			var mapper = new MapKeyMapper(mapping);
			mapper.Column("pizza");
			mapper.Length(50);

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
			mapping.length.Should().Be("50");
		}
	}
}