using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class MapKeyManyToManyMapperColumnsFormulaTest
	{
		[Test]
		public void WhenSetColumnNameThenSetTheName()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);

			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetColumnValuesMoreThanOnesThenMergeColumn()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column("pepe");
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.NotNullable(true));

			mapping.Items.Should().Not.Be.Null();
			var hbmColumn = mapping.Items.OfType<HbmColumn>().First();
			hbmColumn.name.Should().Be("pepe");
			hbmColumn.length.Should().Be("50");
			hbmColumn.notnull.Should().Be(true);
			hbmColumn.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
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
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapper.Executing(x => x.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}

		[Test]
		public void WhenSetOnlyColumnNameThenNoAddColumnTag()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column("pizza");

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
		}

		[Test]
		public void CanSetFormula()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);

			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaThenResetColumn()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column("MyColumn");
			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
			mapping.column.Should().Be.Null();
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithNullThenDoNothing()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column("MyColumn");
			mapper.Formula(null);
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			var formula = @"Line1
Line2";
			mapper.Formula(formula);
			mapping.formula.Should().Be.Null();
			mapping.Items.FirstOrDefault().Should().Not.Be.Null().And.Be.OfType<HbmFormula>();
			var hbmFormula = (HbmFormula)(mapping.Items.First());
			hbmFormula.Text.Length.Should().Be(2);
			hbmFormula.Text[0].Should().Be("Line1");
			hbmFormula.Text[1].Should().Be("Line2");
		}

		[Test]
		public void SettingFormulaOverridesColumn()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column(cm => cm.Unique(true));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnPlainValues()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column("colName");
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingColumnPlainValuesOverridesFormula()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Formula("formula");
			mapper.Column("colName");
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Be("colName");
		}

		[Test]
		public void SettingColumnOverridesFormula()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Formula("formula");
			mapper.Column(cm => cm.Unique(true));
			mapping.formula.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnNodes()
		{
			var mapping = new HbmMapKeyManyToMany();
			var mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Column(cm => cm.SqlType("VARCHAR(80)"));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}
	}
}