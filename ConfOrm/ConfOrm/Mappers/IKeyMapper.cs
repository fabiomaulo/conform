using System.Reflection;

namespace ConfOrm.Mappers
{
	public enum OnDeleteAction
	{
		NoAction,
		Cascade,
	}

	public interface IKeyMapper
	{
		void Column(string columnName);
		void OnDelete(OnDeleteAction deleteAction);
		void PropertyRef(MemberInfo property);
	}

	// TODO : implement IKeyMapper<TEntity> to support strongly typed PropertyRef
}