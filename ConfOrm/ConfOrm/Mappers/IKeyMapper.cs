using System;
using System.Linq.Expressions;
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

	public interface IKeyMapper<TEntity> where TEntity: class
	{
		void Column(string columnName);
		void OnDelete(OnDeleteAction deleteAction);
		void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter);
	}
}