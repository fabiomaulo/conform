using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface IPropertyMapper : IEntityPropertyMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>() where TPersistentType: IUserType;
		void Type<TPersistentType>(object parameters) where TPersistentType : IUserType;
	}
}