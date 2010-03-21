using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IPropertyContainerMapper : IPlainPropertyContainerMapper
	{
		void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping,
		         Action<ICollectionElementRelation> mapping);

		void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping,
		                   Action<ICollectionElementRelation> mapping);

		void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping,
		                    Action<ICollectionElementRelation> mapping);

		void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping,
		                         Action<ICollectionElementRelation> mapping);
	}

	public interface IPropertyContainerMapper<TEntity> : IPlainPropertyContainerMapper<TEntity>
	{
		void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<ISetPropertiesMapper> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<IBagPropertiesMapper> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
											 Action<IListPropertiesMapper> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
		void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
											 Action<IMapPropertiesMapper> collectionMapping,
											 Action<ICollectionElementRelation<TElement>> mapping);
	}
}