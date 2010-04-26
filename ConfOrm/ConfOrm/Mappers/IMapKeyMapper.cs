using System;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface IMapKeyMapper : IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>() where TPersistentType : IUserType;
		void Type(Type persistentType);
		void Length(int length);
	}
}