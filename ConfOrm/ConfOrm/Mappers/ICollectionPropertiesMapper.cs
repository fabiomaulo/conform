using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface ICollectionPropertiesMapper
	{
		void Inverse(bool value);
		void Mutable(bool value);
		void Where(string sqlWhereClause);
		void BatchSize(int value);
		void Lazy(CollectionLazy collectionLazy);
		void Key(Action<IKeyMapper> keyMapping);
		void OrderBy(MemberInfo property);
		void Sort();
		void Cascade(Cascade cascadeStyle);
	}

	public interface ICollectionPropertiesMapper<TEntity, TElement> : ICollectionPropertiesMapper where TEntity : class
	{
		void OrderBy<TProperty>(Expression<Func<TElement, TProperty>> property);
	}

}