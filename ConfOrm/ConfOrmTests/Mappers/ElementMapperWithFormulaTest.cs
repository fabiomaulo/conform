using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class ElementMapperWithFormulaTest
	{
		[Test]
		public void CanSetFormula()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);

			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaThenResetColumn()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column("MyColumn");
			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
			mapping.column.Should().Be.Null();
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithNullThenDoNothing()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column("MyColumn");
			mapper.Formula(null);
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
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
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column(cm => cm.Unique(true));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnPlainValues()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column("colName");
			mapper.Length(10);
			mapper.NotNullable(true);
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.length.Should().Be(null);
			mapping.notnull.Should().Be(false);
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingColumnPlainValuesOverridesFormula()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Formula("formula");
			mapper.Column("colName");
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Be("colName");
		}

		[Test]
		public void SettingColumnOverridesFormula()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Formula("formula");
			mapper.Column(cm => cm.Unique(true));
			mapping.formula.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnNodes()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column(cm => cm.SqlType("VARCHAR(80)"));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}
	}
}