using System;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Properties;
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
	}
}