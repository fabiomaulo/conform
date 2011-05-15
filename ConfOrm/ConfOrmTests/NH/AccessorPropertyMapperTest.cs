using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class AccessorPropertyMapperTest
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
			public string ReadOnly { get { return ""; } }

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

		private class Movement<TDetail>
		{
			private ISet<TDetail> _details;
			public IEnumerable<TDetail> Details
			{
				get { return _details; }
			}
		}

		private class MovementDetail<TMovement>
		{
			public TMovement Movement { get; set; }
		}

		private class Income : Movement<IncomeDetail> { }

		private class IncomeDetail : MovementDetail<Income> { }

		[Test]
		public void WhenNoMemberThenAccessNone()
		{
			string accessValue = null;

			new AccessorPropertyMapper(typeof (MyClass), null, x => accessValue = x);

			accessValue.Should().Be.EqualTo("none");
		}

		[Test]
		public void WhenNoMemberThenCantChangeAccessor()
		{
			string accessValue = null;

			var mapper = new AccessorPropertyMapper(typeof(MyClass), null, x => accessValue = x);
			mapper.Access(Accessor.Field);
			accessValue.Should().Be.EqualTo("none");
		}

		[Test]
		public void WhenMapFieldAutoAssignAccessField()
		{
			string accessValue = null;

			new AccessorPropertyMapper(typeof(MyClass), "aField", x => accessValue = x);

			accessValue.Should().Be.EqualTo("field");
		}

		[Test]
		public void WhenMapPropertyNoAutoAssignAccessField()
		{
			string accessValue = null;

			new AccessorPropertyMapper(typeof(MyClass), "Autoproperty", x => accessValue = x);

			accessValue.Should().Be.Null();
		}

		[Test]
		public void WhenMapPropertyWithFieldPascalcaseUnderscoreThenChooseRigthFieldAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(MyClass), "WithFieldPascalcaseUnderscore", x => accessValue = x);

			mapper.Access(Accessor.Field);

			accessValue.Should().Be.EqualTo("field.pascalcase-underscore");
		}

		[Test]
		public void WhenMapPropertyWithFieldCamelCaseThenChooseRigthFieldAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(MyClass), "WithFieldCamelCase", x => accessValue = x);

			mapper.Access(Accessor.NoSetter);

			accessValue.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenMapPropertyNosetterPascalcaseUnderscoreThenChooseRigthFieldAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(MyClass), "NosetterPascalcaseUnderscore", x => accessValue = x);

			mapper.Access(Accessor.NoSetter);

			accessValue.Should().Be.EqualTo("nosetter.pascalcase-underscore");
		}

		[Test]
		public void WhenMapPropertyNosetterCamelCaseThenChooseRigthFieldAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(MyClass), "NosetterCamelCase", x => accessValue = x);

			mapper.Access(Accessor.NoSetter);

			accessValue.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenMapReadOnlyPropertyThenReadonlyAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(MyClass), "ReadOnly", x => accessValue = x);

			mapper.Access(Accessor.ReadOnly);

			accessValue.Should().Be.EqualTo("readonly");
		}

		[Test]
		public void WhenMapPropertyWithFieldOnBaseClassThenChooseRigthFieldAccessor()
		{
			string accessValue = null;
			var mapper = new AccessorPropertyMapper(typeof(Income), "Details", x => accessValue = x);

			mapper.Access(Accessor.Field);

			accessValue.Should().Be.EqualTo("field.camelcase-underscore");
		}
	}
}