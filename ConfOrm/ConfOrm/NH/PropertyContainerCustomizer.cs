using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class PropertyContainerCustomizer<TEntity> where TEntity : class
	{
		public PropertyContainerCustomizer(ICustomizersHolder customizersHolder)
		{
			CustomizersHolder = customizersHolder;
		}

		protected ICustomizersHolder CustomizersHolder { get; private set; }

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, mapping);
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property,
		                                  Action<IComponentMapper<TComponent>> mapping) where TComponent : class
		{
			mapping(new ComponentCustomizer<TComponent>(CustomizersHolder));
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, mapping);
		}

		public void OneToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IOneToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, mapping);
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                          Action<ISetPropertiesMapper> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, collectionMapping);
		}

		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                          Action<IBagPropertiesMapper> collectionMapping,
		                          Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, collectionMapping);
		}

		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
		                           Action<IListPropertiesMapper> collectionMapping,
		                           Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, collectionMapping);
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
		                                Action<IMapPropertiesMapper> collectionMapping,
		                                Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(member, collectionMapping);
		}
	}
}