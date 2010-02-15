using System;
using System.Linq;
using System.Data;
using System.Reflection;
using ConfOrm.Mappers;
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
			private string aField;
			private string withFieldCamelCase;
			public string WithFieldCamelCase
			{
				get { return withFieldCamelCase; }
				set { withFieldCamelCase = value; }
			}
			private string _WithFieldPascalcaseUnderscore;
			public string WithFieldPascalcaseUnderscore
			{
				get { return _WithFieldPascalcaseUnderscore; }
				set { _WithFieldPascalcaseUnderscore = value; }
			}

			public string Autoproperty { get; set; }
			public string ReadOnly { get { return "";}}

			private string nosetterCamelCase;
			public string NosetterCamelCase
			{
				get { return nosetterCamelCase; }
			}
			private string _NosetterPascalcaseUnderscore;
			public string NosetterPascalcaseUnderscore
			{
				get { return _NosetterPascalcaseUnderscore; }
			}

		}

		[Test]
		public void WhenNoMemberThenAccessNone()
		{
			var mapping = new HbmProperty();

			new PropertyMapper(null, mapping);

			mapping.Access.Should().Be.EqualTo("none");
		}

		[Test]
		public void WhenMapFieldAutoAssignAccessField()
		{
			var member = typeof (MyClass).GetField("aField", BindingFlags.Instance | BindingFlags.NonPublic);
			var mapping = new HbmProperty();
			
			new PropertyMapper(member, mapping);

			mapping.Access.Should().Be.EqualTo("field");
		}

		[Test]
		public void WhenMapPropertyNoAutoAssignAccessField()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();

			new PropertyMapper(member, mapping);

			mapping.Access.Should().Be.Null();
		}

		[Test]
		public void WhenMapPropertyWithFieldPascalcaseUnderscoreThenChooseRigthFieldAccessor()
		{
			var member = typeof(MyClass).GetProperty("WithFieldPascalcaseUnderscore");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			
			mapper.Access(Accessor.Field);

			mapping.Access.Should().Be.EqualTo("field.pascalcase-underscore");
		}

		[Test]
		public void WhenMapPropertyWithFieldCamelCaseThenChooseRigthFieldAccessor()
		{
			var member = typeof(MyClass).GetProperty("WithFieldCamelCase");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Access(Accessor.NoSetter);

			mapping.Access.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenMapPropertyNosetterPascalcaseUnderscoreThenChooseRigthFieldAccessor()
		{
			var member = typeof(MyClass).GetProperty("NosetterPascalcaseUnderscore");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Access(Accessor.NoSetter);

			mapping.Access.Should().Be.EqualTo("nosetter.pascalcase-underscore");
		}

		[Test]
		public void WhenMapPropertyNosetterCamelCaseThenChooseRigthFieldAccessor()
		{
			var member = typeof(MyClass).GetProperty("NosetterCamelCase");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Access(Accessor.NoSetter);

			mapping.Access.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenMapReadOnlyPropertyThenReadonlyAccessor()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Access(Accessor.ReadOnly);

			mapping.Access.Should().Be.EqualTo("readonly");
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