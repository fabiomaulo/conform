using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.UserTypes;

namespace ConfOrm.Mappers
{
	public interface ICollectionPropertiesMapper: IEntityPropertyMapper
	{
		void Inverse(bool value);
		void Mutable(bool value);
		void Where(string sqlWhereClause);
		void BatchSize(int value);
		void Lazy(CollectionLazy collectionLazy);
		void Key(Action<IKeyMapper> keyMapping);
		void OrderBy(MemberInfo property);
		void Sort();
		void Sort<TComparer>();
		void Cascade(Cascade cascadeStyle);
		//void Type(string namedCollectionType); // TODO: figure out a way to avoid string for embedded namedCollectionType
		void Type<TCollection>() where TCollection: IUserCollectionType;
		void Type(Type collectionType);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
	}

	public interface ICollectionPropertiesMapper<TElement> : ICollectionPropertiesMapper
	{
		void OrderBy<TProperty>(Expression<Func<TElement, TProperty>> property);
	}

}