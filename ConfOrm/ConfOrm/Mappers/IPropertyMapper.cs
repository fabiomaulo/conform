using System;
using NHibernate.Type;

namespace ConfOrm.Mappers
{
	public interface IPropertyMapper : IEntityPropertyMapper, IColumnsMapper
	{
		void Type(IType persistentType);
		void Type<TPersistentType>();
		void Type<TPersistentType>(object parameters);
		void Type(Type persistentType, object parameters);
		void Length(int length);
		void Precision(short precision);
		void Scale(short scale);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
		void Formula(string formula);
	}
}