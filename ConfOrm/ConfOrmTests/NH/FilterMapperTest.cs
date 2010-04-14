using System;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class FilterMapperTest
	{
		[Test]
		public void CantCreateWithEmptyName()
		{
			Executing.This(() => new FilterMapper(null, new HbmFilter())).Should().Throw().Exception.Should().Be.InstanceOf<ArgumentException>();
			Executing.This(() => new FilterMapper("", new HbmFilter())).Should().Throw().Exception.Should().Be.InstanceOf<ArgumentException>();
			Executing.This(() => new FilterMapper("   ", new HbmFilter())).Should().Throw().Exception.Should().Be.InstanceOf<ArgumentException>();
		}

		[Test]
		public void CantCreateWithNullHbmFilter()
		{
			Executing.This(() => new FilterMapper("MyFilter", null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCreatedForceTheNameOfTheFilter()
		{
			var hbmFilter = new HbmFilter();
			new FilterMapper("MyFilter", hbmFilter);
			hbmFilter.name.Should().Be("MyFilter");
		}

		[Test]
		public void WhenSetSingleLineConditionThenSetSimpleCondition()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition("aFiled = :aParameter");
			hbmFilter.condition.Should().Be("aFiled = :aParameter");
			hbmFilter.Text.Should().Be.Null();
		}

		[Test]
		public void WhenSetMultiLineConditionThenSetTextCondition()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition("aFiled = :aParameter" + Environment.NewLine + "AND anotherField = :anotherParam");
			hbmFilter.condition.Should().Be.Null();
			hbmFilter.Text.Should().Not.Be.Null();
			hbmFilter.Text.Should().Have.SameSequenceAs("aFiled = :aParameter", "AND anotherField = :anotherParam");
		}

		[Test]
		public void WhenSetSingleLineConditionThenResetTextCondition()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition("aFiled = :aParameter" + Environment.NewLine + "AND anotherField = :anotherParam");
			mapper.Condition("aFiled = :aParameter");
			mapper.Condition("aFiled = :aParameter");
			hbmFilter.Text.Should().Be.Null();
		}

		[Test]
		public void WhenSetMultiLineConditionThenResetSimpleCondition()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition("aFiled = :aParameter");
			mapper.Condition("aFiled = :aParameter" + Environment.NewLine + "AND anotherField = :anotherParam");
			hbmFilter.condition.Should().Be.Null();
			hbmFilter.Text.Should().Have.SameSequenceAs("aFiled = :aParameter", "AND anotherField = :anotherParam");
		}

		[Test]
		public void WhenSetNullConditionThenSetBothConditionToNull()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition(null);
			hbmFilter.condition.Should().Be.Null();
			hbmFilter.Text.Should().Be.Null();
		}

		[Test]
		public void WhenSetEmptyConditionThenSetBothConditionToNull()
		{
			var hbmFilter = new HbmFilter();
			var mapper = new FilterMapper("MyFilter", hbmFilter);
			mapper.Condition(string.Empty);
			hbmFilter.condition.Should().Be.Null();
			hbmFilter.Text.Should().Be.Null();

			mapper.Condition("    ");
			hbmFilter.condition.Should().Be.Null();
			hbmFilter.Text.Should().Be.Null();
		}
	}
}