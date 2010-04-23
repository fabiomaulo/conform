using System;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ManyToManyMapperTest
	{
		private interface IMyInterface
		{

		}
		private class MyClass : IMyInterface
		{

		}
		private class Whatever
		{

		}
		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.Column(cm => cm.Name("MyClass"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
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
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
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
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));

			mapper.Executing(x => x.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}

		[Test]
		public void CanAssignNotFoundMode()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.NotFound(NotFoundMode.Ignore);
			mapping.NotFoundMode.Should().Be(HbmNotFoundMode.Ignore);
		}

		[Test]
		public void CanForceClassRelation()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);

			mapper.Class(typeof(MyClass));

			mapping.Class.Should().Contain("MyClass").And.Not.Contain("IMyInterface");
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);

			Executing.This(() => mapper.Class(typeof(Whatever))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanAssignEntityName()
		{
			var mapping = new HbmManyToMany();
			var mapper = new ManyToManyMapper(typeof(MyClass), mapping, null);
			mapper.EntityName("myname");
			mapping.EntityName.Should().Be("myname");
		}
	}
}