using System;
using System.Data;
using System.Linq;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ElementMapperTest
	{
		private enum MyEnum
		{
			One
		}
		[Test]
		public void WhenCreatedMustHaveAllParametersValid()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new ElementMapper(null, null));
			ActionAssert.Throws<ArgumentNullException>(() => new ElementMapper(typeof(string), null));
			ActionAssert.Throws<ArgumentNullException>(() => new ElementMapper(null, new HbmElement()));
		}

		[Test]
		public void WhenCreatedThenAutoAssignType()
		{
			var elementMapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), elementMapping);
			elementMapping.type1.Should().Be("String");
		}

		[Test]
		public void WhenCreatedWithEnumThenNoAutoAssignType()
		{
			// Note: the responsibility of the type name is delegated to NHibernate: NHibernateUtil.Enum(typeClass)
			var elementMapping = new HbmElement();
			new ElementMapper(typeof(MyEnum), elementMapping);
			elementMapping.type1.Should().Not.Be.Empty();
		}

		[Test]
		public void WhenSetTypeByITypeThenSetTypeName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Type(NHibernateUtil.Int64);

			mapping.Type.name.Should().Be.EqualTo("Int64");
		}

		[Test]
		public void WhenSetTypeByIUserTypeThenSetTypeName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Type<MyType>();

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetTypeByNotFormalITypeThenSetTypeFullName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Type<EnumStringType<MyEnum>>();

			mapping.Type.name.Should().Contain(typeof(EnumStringType<MyEnum>).FullName);
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithParamsThenSetType()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Type<MyType>(new { Param1 = "a", Param2 = 12 });

			mapping.type1.Should().Be.Null();
			mapping.Type.name.Should().Contain("MyType");
			mapping.Type.param.Should().Have.Count.EqualTo(2);
			mapping.Type.param.Select(p => p.name).Should().Have.SameValuesAs("Param1", "Param2");
			mapping.Type.param.Select(p => p.GetText()).Should().Have.SameValuesAs("a", "12");
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithNullParamsThenSetTypeName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Type<MyType>(null);

			mapping.Type.name.Should().Contain("MyType");
			mapping.type.Should().Be.Null();
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			ActionAssert.Throws<ArgumentOutOfRangeException>(() => mapper.Type(typeof(object), null));
			ActionAssert.Throws<ArgumentNullException>(() => mapper.Type(null, null));
		}

		[Test]
		public void WhenSetColumnNameThenSetTheName()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column(cm =>
			{
				cm.Length(50);
				cm.NotNullable(true);
			});
			mapping.Items.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.notnull.Should().Be(true);
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
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
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.NotNullable(true));

			mapping.Items.Should().Be.Null();
			mapping.length.Should().Be("50");
			mapping.notnull.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
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
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			ActionAssert.Throws<ConfOrm.MappingException>(() => mapper.Column(cm => cm.Length(50)));
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var mapping = new HbmElement();
			var mapper = new ElementMapper(typeof(string), mapping);
			mapper.Column("pizza");
			mapper.Length(50);
			mapper.Precision(10);
			mapper.Scale(2);
			mapper.NotNullable(true);
			mapper.Unique(true);

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
			mapping.length.Should().Be("50");
			mapping.precision.Should().Be("10");
			mapping.scale.Should().Be("2");
			mapping.notnull.Should().Be(true);
			mapping.unique.Should().Be(true);
		}

		public class MyType : IUserType
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
}