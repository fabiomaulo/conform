using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface IVersionMapper : IAccessorPropertyMapper, IColumnsMapper
	{
		void Type(IVersionType persistentType);
		void Type<TPersistentType>() where TPersistentType : IUserVersionType;
		void Type<TPersistentType>(object parameters) where TPersistentType : IUserVersionType;
		void Type(System.Type persistentType, object parameters);
		void UnsavedValue(object value);
		void Insert(bool useInInsert);
		void Generated(VersionGeneration generatedByDb);
	}
}