using System;
using System.Linq;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Properties;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class KeyPropertyMapperTest
	{
				private class MyClass
		{
			public string Autoproperty { get; set; }
			public string ReadOnly { get { return "";}}
		}

		[Test]
		public void WhenSettingByTypeThenCheckCompatibility()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);

			mapper.Executing(m=> m.Access(typeof(object))).Throws<ArgumentOutOfRangeException>();
			mapper.Executing(m => m.Access(typeof(FieldAccessor))).NotThrows();
			mapping.Access.Should().Be.EqualTo(typeof(FieldAccessor).AssemblyQualifiedName);
		}

		[Test]
		public void WhenSetTypeByITypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Type(NHibernateUtil.String);

			mapping.Type.name.Should().Be.EqualTo("String");
		}

		[Test]
		public void WhenSetTypeByIUserTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Type<MyType>();

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithParamsThenSetType()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Type<MyType>(new { Param1="a", Param2=12 });

			mapping.type1.Should().Be.Null();
			mapping.Type.name.Should().Contain("MyType");
			mapping.Type.param.Should().Have.Count.EqualTo(2);
			mapping.Type.param.Select(p => p.name).Should().Have.SameValuesAs("Param1","Param2");
			mapping.Type.param.Select(p => p.GetText()).Should().Have.SameValuesAs("a", "12");
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithNullParamsThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Type<MyType>(null);

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Executing(m=> m.Type(typeof(object), null)).Throws<ArgumentOutOfRangeException>();
			mapper.Executing(m => m.Type(null, null)).Throws<ArgumentNullException>();
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Column(cm => { cm.Name("Autoproperty"); cm.Length(50); });
			mapping.column.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Column(cm => cm.Length(50));
			mapping.column.Should().Be.Null();
			mapping.length.Should().Be("50");
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			mapping.column.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnesThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.Name("Pizza"));

			mapping.column.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.column1.Should().Be("Pizza");
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
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
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n=> !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmKeyProperty();
			var mapper = new KeyPropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapper.Executing(m => m.Column(cm => cm.Length(50))).Throws<ConfOrm.MappingException>();
		}
	}
}