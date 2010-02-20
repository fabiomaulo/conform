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
	}
}