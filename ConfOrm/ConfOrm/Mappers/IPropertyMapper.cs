using System;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface IPropertyMapper : IEntityPropertyMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>() where TPersistentType: IUserType;
		void Type<TPersistentType>(object parameters) where TPersistentType : IUserType;
		void Type(System.Type persistentType, object parameters);
		void Column(Action<IColumnMapper> columnMapper);
		void Columns(params Action<IColumnMapper>[] columnMapper);
		void Column(string name);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
	}
}