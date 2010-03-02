using System;
using System.Linq;
using System.Data;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Properties;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PropertyMapperTest
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
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Access(typeof(object)));
			ActionAssert.NotThrow(() => mapper.Access(typeof(FieldAccessor)));
			mapping.Access.Should().Be.EqualTo(typeof(FieldAccessor).AssemblyQualifiedName);
		}

		[Test]
		public void WhenSetTypeByITypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type(NHibernateUtil.String);

			mapping.Type.name.Should().Be.EqualTo("String");
		}

		[Test]
		public void WhenSetTypeByIUserTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>();

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithParamsThenSetType()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
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
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>(null);

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			ActionAssert.Throws<ArgumentOutOfRangeException>(()=> mapper.Type(typeof(object), null));
			ActionAssert.Throws<ArgumentNullException>(() => mapper.Type(null, null));
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => { cm.Name("Autoproperty"); cm.Length(50); });
			mapping.column.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => { cm.Length(50);
			                    	cm.NotNullable(true); });
			mapping.Items.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnesThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.NotNullable(true));

			mapping.Items.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
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
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n=> !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			ActionAssert.Throws<ConfOrm.MappingException>(() => mapper.Column(cm => cm.Length(50)));
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column("pizza");
			mapper.Length(50);
			mapper.Precision(10);
			mapper.Scale(2);
			mapper.NotNullable(true);
			mapper.Unique(true);
			mapper.UniqueKey("AA");
			mapper.Index("II");

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
			mapping.length.Should().Be("50");
			mapping.precision.Should().Be("10");
			mapping.scale.Should().Be("2");
			mapping.notnull.Should().Be(true);
			mapping.unique.Should().Be(true);
			mapping.uniquekey.Should().Be("AA");
			mapping.index.Should().Be("II");
		}
	}

	public class MyType: IUserType
	{
		#region Implementation of IUserType

		public bool Equals(object x, object y)
		{
			throw new NotImplementedException();
		}

		public int GetHashCode(object x)
		{
			throw new NotImplementedException();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			throw new NotImplementedException();
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			throw new NotImplementedException();
		}

		public object DeepCopy(object value)
		{
			throw new NotImplementedException();
		}

		public object Replace(object original, object target, object owner)
		{
			throw new NotImplementedException();
		}

		public object Assemble(object cached, object owner)
		{
			throw new NotImplementedException();
		}

		public object Disassemble(object value)
		{
			throw new NotImplementedException();
		}

		public SqlType[] SqlTypes
		{
			get { throw new NotImplementedException(); }
		}

		public Type ReturnedType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}