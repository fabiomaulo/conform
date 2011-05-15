using System;
using System.Collections.Generic;
using System.Data;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class CollectionElementCustomizerTest
	{
		private class MyClass
		{
			public IEnumerable<string> MyCollection { get; set; }
		}

		private class MyUserType: IUserType
		{
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
		}
		[Test]
		public void InvokeDirectMethods()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionElementCustomizer(propertyPath, customizersHolder);
			var elementMapper = new Mock<IElementMapper>();

			customizer.Length(10);
			customizer.Precision(5);
			customizer.Scale(2);
			customizer.NotNullable(true);
			customizer.Unique(true);
			customizer.Column("pizza");
			customizer.Type(NHibernateUtil.StringClob);
			customizer.Type<MyUserType>();
			var parameters = new {something= 10};
			customizer.Type<MyUserType>(parameters);
			customizer.Type(typeof(MyUserType), parameters);

			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Length(It.Is<int>(v => v == 10)), Times.Once());
			elementMapper.Verify(x => x.Precision(It.Is<short>(v => v == 5)), Times.Once());
			elementMapper.Verify(x => x.Scale(It.Is<short>(v => v == 2)), Times.Once());
			elementMapper.Verify(x => x.Unique(It.Is<bool>(v => v)), Times.Once());
			elementMapper.Verify(x => x.NotNullable(It.Is<bool>(v => v)), Times.Once());
			elementMapper.Verify(x => x.Column(It.Is<string>(v => v == "pizza")), Times.Once());
			elementMapper.Verify(x => x.Type(It.Is<IType>(v => v.GetType() == NHibernateUtil.StringClob.GetType())), Times.Once());
			elementMapper.Verify(x => x.Type<MyUserType>(), Times.Once());
			elementMapper.Verify(x => x.Type<MyUserType>(It.Is<object>(v=> v == parameters)), Times.Once());
			elementMapper.Verify(x => x.Type(It.Is<Type>(v=> v == typeof(MyUserType)), It.Is<object>(v => v == parameters)), Times.Once());
		}

		[Test]
		public void InvokeColumnCustomizer()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new CollectionElementCustomizer(propertyPath, customizersHolder);
			var elementMapper = new Mock<IElementMapper>();
			var columnMapper = new Mock<IColumnMapper>();
			elementMapper.Setup(x => x.Column(It.IsAny<Action<IColumnMapper>>())).Callback<Action<IColumnMapper>>(
				x => x.Invoke(columnMapper.Object));

			customizer.Column(c => c.SqlType("VARCHAR(100)"));
			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			columnMapper.Verify(x => x.SqlType(It.Is<string>(v => v == "VARCHAR(100)")));
		}
	}
}