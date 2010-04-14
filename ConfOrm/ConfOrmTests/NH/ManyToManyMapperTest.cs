using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ManyToManyMapperTest
	{
		private class MyClass
		{
		}
		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
			mapper.Column(cm => cm.Name("MyClass"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(3)");
				cm.NotNullable(true);
			});
			mapping.Columns.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
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
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));

			mapper.Executing(x => x.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}
	}
}