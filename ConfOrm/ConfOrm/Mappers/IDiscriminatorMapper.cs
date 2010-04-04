using System;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface IDiscriminatorMapper
	{
		void Column(string column);
		void Column(Action<IColumnMapper> columnMapper);
		void Type(IType persistentType);
		void Type<TPersistentType>() where TPersistentType : IUserType;
		void Type(Type persistentType);
		void Formula(string formula);
		void Force(bool force);
		void Insert(bool applyOnApplyOnInsert);
		void NotNullable(bool isNotNullable);
		void Length(int length);
	}
}