using System;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class DiscriminatorMapperTest
	{
		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new DiscriminatorMapper(null));
		}

		[Test]
		public void CanSetFormula()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Formula("SomeFormula");
			hbmDiscriminator.formula.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaThenResetColumn()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column("MyColumn");
			mapper.Formula("SomeFormula");
			hbmDiscriminator.formula.Should().Be("SomeFormula");
			hbmDiscriminator.column.Should().Be.Null();
			hbmDiscriminator.Item.Should().Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithNullThenDoNothing()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column("MyColumn");
			mapper.Formula(null);
			hbmDiscriminator.formula.Should().Be.Null();
			hbmDiscriminator.column.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			var formula = @"Line1
Line2";
			mapper.Formula(formula);
			hbmDiscriminator.formula.Should().Be.Null();
			hbmDiscriminator.Item.Should().Not.Be.Null().And.Be.OfType<HbmFormula>();
			var hbmFormula = (HbmFormula) (hbmDiscriminator.Item);
			hbmFormula.Text.Length.Should().Be(2);
			hbmFormula.Text[0].Should().Be("Line1");
			hbmFormula.Text[1].Should().Be("Line2");
		}

		[Test]
		public void CanSetForce()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			
			mapper.Force(true);
			hbmDiscriminator.force.Should().Be(true);
			
			mapper.Force(false);
			hbmDiscriminator.force.Should().Be(false);
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			
			mapper.Insert(true);
			hbmDiscriminator.insert.Should().Be(true);

			mapper.Insert(false);
			hbmDiscriminator.insert.Should().Be(false);
		}

		[Test]
		public void CanSetNotNullable()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);

			mapper.NotNullable(true);
			hbmDiscriminator.notnull.Should().Be(true);

			mapper.NotNullable(false);
			hbmDiscriminator.notnull.Should().Be(false);
		}

		[Test]
		public void CanSetLength()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);

			mapper.Length(77);
			hbmDiscriminator.length.Should().Be("77");
		}

		[Test]
		public void WhenSetTypeByITypeThenSetTypeName()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Type(NHibernateUtil.String);
			hbmDiscriminator.type.Should().Be.EqualTo("String");
		}

		[Test]
		public void WhenSetTypeByIDiscriminatorThenSetTypeName()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Type(NHibernateUtil.String);
			hbmDiscriminator.type.Should().Contain("String");
		}

		[Test]
		public void WhenSetTypeByGenericMethodThenSetTypeName()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Type<EnumStringType<MyEnum>>();
			hbmDiscriminator.type.Should().Contain(typeof(EnumStringType<MyEnum>).FullName);
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Type(typeof(object)));
			ActionAssert.Throws<ArgumentNullException>(() => mapper.Type((Type)null));
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column(cm =>
			{
				cm.Length(50);
				cm.NotNullable(true);
			});
			hbmDiscriminator.Item.Should().Be.Null();
			hbmDiscriminator.length.Should().Be("50");
			hbmDiscriminator.notnull.Should().Be(true);
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			hbmDiscriminator.Item.Should().Not.Be.Null();
			hbmDiscriminator.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnceThenMergeColumn()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.NotNullable(true));

			hbmDiscriminator.Item.Should().Be.Null();
			hbmDiscriminator.length.Should().Be("50");
			hbmDiscriminator.notnull.Should().Be(true);
		}

		[Test]
		public void SettingFormulaOverridesColumn()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column(cm => cm.Unique(true));
			mapper.Formula("formula");
			hbmDiscriminator.formula.Should().Be("formula");
			hbmDiscriminator.Item.Should().Be.Null();
		}

		[Test]
		public void SettingFormulaOverridesColumnPlainValues()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Column("colName");
			mapper.Length(10);
			mapper.NotNullable(true);
			mapper.Formula("formula");
			hbmDiscriminator.formula.Should().Be("formula");
			hbmDiscriminator.column.Should().Be(null);
			hbmDiscriminator.length.Should().Be(null);
			hbmDiscriminator.notnull.Should().Be(false);
			hbmDiscriminator.Item.Should().Be.Null();
		}

		[Test]
		public void SettingColumnPlainValuesOverridesFormula()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Formula("formula");
			mapper.Column("colName");
			hbmDiscriminator.formula.Should().Be.Null();
			hbmDiscriminator.column.Should().Be("colName");
		}

		[Test]
		public void SettingColumnOverridesFormula()
		{
			var hbmDiscriminator = new HbmDiscriminator();
			var mapper = new DiscriminatorMapper(hbmDiscriminator);
			mapper.Formula("formula");
			mapper.Column(cm => cm.Unique(true));
			hbmDiscriminator.formula.Should().Be.Null();
			hbmDiscriminator.Item.Should().Be.OfType<HbmColumn>();
		}

		private enum MyEnum
		{
			One,
			Two,
			Three
		}
	}
}