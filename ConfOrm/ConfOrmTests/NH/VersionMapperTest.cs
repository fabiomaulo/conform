using System;
using System.Linq;
using System.Data;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Properties;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NUnit.Framework;
using SharpTestsEx;
using MappingException = ConfOrm.MappingException;

namespace ConfOrmTests.NH
{
	public class VersionMapperTest
	{
		private class MyClass
		{
			public int Version { get; set; }
			public int ReadOnly { get { return 0; } }
		}

		[Test]
		public void ShouldSetThePropertyNameImmediately()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			new VersionMapper(member, mapping);

			mapping.name.Should().Be.EqualTo("Version");
		}

		[Test]
		public void WhenSetAccessorByTypeThenCheckCompatibility()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);

			Executing.This(() => mapper.Access(typeof(object))).Should().Throw<ArgumentOutOfRangeException>();
			Executing.This(() => mapper.Access(typeof(FieldAccessor))).Should().NotThrow();
			mapping.access.Should().Be.EqualTo(typeof(FieldAccessor).AssemblyQualifiedName);
		}

		[Test]
		public void WhenSetTypeByIVersionTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Type((IVersionType)NHibernateUtil.Int32);

			mapping.type.Should().Be.EqualTo("Int32");
		}

		[Test]
		public void WhenSetTypeByIUserVersionTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Type<MyVerionType>();

			mapping.type.Should().Contain("MyVerionType");
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			Executing.This(() => mapper.Type(typeof(object))).Should().Throw<ArgumentOutOfRangeException>();
			Executing.This(() => mapper.Type((Type)null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Column(cm => cm.Name("pepe"));

			mapping.Columns.Should().Have.Count.EqualTo(1);
			mapping.Columns.Single().name.Should().Be("pepe");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Column(cm => cm.Name("Version"));
			mapping.column1.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("timestamp");
				cm.NotNullable(true);
			});
			mapping.Columns.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Type<MyVerionType>();
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
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Executing.This(() => mapper.Column(cm => cm.Length(50))).Should().Throw<MappingException>();
		}

		[Test]
		public void CanSetInsert()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Insert(true);
			mapping.insertSpecified.Should("True is the default value (not specified if true)").Be.False();
			mapper.Insert(false);
			mapping.insert.Should().Be.False();
			mapping.insertSpecified.Should().Be.True();
		}

		[Test]
		public void CanSetUsavedValue()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.UnsavedValue(null);
			mapping.unsavedvalue.Should().Be("null");
			mapper.UnsavedValue(0);
			mapping.unsavedvalue.Should().Be("0");
		}

		[Test]
		public void CanSetGenerated()
		{
			var member = typeof(MyClass).GetProperty("Version");
			var mapping = new HbmVersion();
			var mapper = new VersionMapper(member, mapping);
			mapper.Generated(VersionGeneration.Always);
			mapping.generated.Should().Be(HbmVersionGeneration.Always);
		}

		private class MyVerionType: IUserVersionType
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

			#region Implementation of IComparer

			public int Compare(object x, object y)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Implementation of IUserVersionType

			public object Seed(ISessionImplementor session)
			{
				throw new NotImplementedException();
			}

			public object Next(object current, ISessionImplementor session)
			{
				throw new NotImplementedException();
			}

			#endregion
		}	
	}
}
