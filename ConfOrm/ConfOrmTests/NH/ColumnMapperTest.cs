using System;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ColumnMapperTest
	{
		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new ColumnMapper(null, null));
			ActionAssert.Throws<ArgumentNullException>(() => new ColumnMapper(null, "pepe"));
			ActionAssert.Throws<ArgumentNullException>(() => new ColumnMapper(new HbmColumn(), null));
			ActionAssert.Throws<ArgumentNullException>(() => new ColumnMapper(new HbmColumn(), string.Empty));
		}

		[Test]
		public void CanSetName()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			
			hbm.name.Should("be assigned by default").Be("aColumn");
			
			mapper.Name("pizza");
			hbm.name.Should().Be("pizza");
		}

		[Test]
		public void CanSetLength()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			mapper.Length(50);
			
			hbm.length.Should().Be("50");

			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Length(0));
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Length(-1));
		}

		[Test]
		public void CanSetPrecision()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			mapper.Precision(12);

			hbm.precision.Should().Be("12");
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Precision(0));
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Precision(-1));
		}

		[Test]
		public void CanSetScale()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			mapper.Scale(3);

			hbm.scale.Should().Be("3");
			ActionAssert.NotThrow(() => mapper.Scale(0));
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Scale(-1));
		}

		[Test]
		public void CanSetNotNullable()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			mapper.NotNullable(true);

			hbm.notnull.Should().Be(true);
			hbm.notnullSpecified.Should().Be(true);

			mapper.NotNullable(false);

			hbm.notnull.Should().Be(false);
			hbm.notnullSpecified.Should().Be(false);
		}

		[Test]
		public void CanSetUnique()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");

			mapper.Unique(true);

			hbm.unique.Should().Be(true);
			hbm.uniqueSpecified.Should().Be(true);

			mapper.Unique(false);

			hbm.unique.Should().Be(false);
			hbm.uniqueSpecified.Should().Be(false);
		}

		[Test]
		public void CanSetUniqueKey()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");

			mapper.UniqueKey("pizza");
			hbm.uniquekey.Should().Be("pizza");
		}

		[Test]
		public void CanSetSqlType()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");
			
			mapper.SqlType("NVARCHAR(123)");
			hbm.sqltype.Should().Be("NVARCHAR(123)");
		}

		[Test]
		public void CanSetIndex()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");

			mapper.Index("pizza");
			hbm.index.Should().Be("pizza");
		}

		[Test]
		public void CanSetCheck()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");

			mapper.Check("pizza");
			hbm.check.Should().Be("pizza");
		}

		[Test]
		public void CanSetDefault()
		{
			var hbm = new HbmColumn();
			var mapper = new ColumnMapper(hbm, "aColumn");

			mapper.Default("pizza");
			hbm.@default.Should().Be("pizza");

			mapper.Default(123);
			hbm.@default.Should().Be("123");

			mapper.Default(12.3);
			hbm.@default.Should().Be("12.3");

			mapper.Default(null);
			hbm.@default.Should().Be("null");
		}
	}
}