using System.Linq;
using ConfOrm;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PropertyMapperWithFormulaTest
	{
		private class MyClass
		{
			public string Autoproperty { get; set; }
		}

		[Test]
		public void CanSetFormula()
		{
			var member = ForClass<MyClass>.Property(c=> c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaThenResetColumn()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column("MyColumn");
			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
			mapping.column.Should().Be.Null();
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithNullThenDoNothing()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column("MyColumn");
			mapper.Formula(null);
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
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
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => cm.Unique(true));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnPlainValues()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
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
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Formula("formula");
			mapper.Column("colName");
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Be("colName");
		}

		[Test]
		public void SettingColumnOverridesFormula()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Formula("formula");
			mapper.Column(cm => cm.Unique(true));
			mapping.formula.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnNodes()
		{
			var member = ForClass<MyClass>.Property(c => c.Autoproperty);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm=> cm.SqlType("VARCHAR(80)"));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}
	}
}