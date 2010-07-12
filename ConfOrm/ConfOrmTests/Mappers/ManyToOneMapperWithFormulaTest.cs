using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Mappers
{
	public class ManyToOneMapperWithFormulaTest
	{
		private class MyClass
		{
			public Relation Relation { get; set; }
		}

		private class Relation
		{

		}

		[Test]
		public void CanSetFormula()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c=> c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaThenResetColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column("MyColumn");
			mapper.Formula("SomeFormula");
			mapping.formula.Should().Be("SomeFormula");
			mapping.column.Should().Be.Null();
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithNullThenDoNothing()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column("MyColumn");
			mapper.Formula(null);
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
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
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.Unique(true));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnPlainValues()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column("colName");
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}

		[Test]
		public void SettingColumnPlainValuesOverridesFormula()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Formula("formula");
			mapper.Column("colName");
			mapping.formula.Should().Be.Null();
			mapping.column.Should().Be("colName");
		}

		[Test]
		public void SettingColumnOverridesFormula()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Formula("formula");
			mapper.Column(cm => cm.Unique(true));
			mapping.formula.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnNodes()
		{
			var hbmMapping = new HbmMapping();
			var member = ForClass<MyClass>.Property(c => c.Relation);
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.SqlType("VARCHAR(80)"));
			mapper.Formula("formula");
			mapping.formula.Should().Be("formula");
			mapping.column.Should().Be(null);
			mapping.Items.Should().Be.Null();
		}
	}
}