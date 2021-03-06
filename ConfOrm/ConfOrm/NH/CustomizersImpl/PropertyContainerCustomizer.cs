using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace ConfOrm.NH.CustomizersImpl
{
	public class PropertyContainerCustomizer<TEntity> where TEntity : class
	{
		public PropertyContainerCustomizer(ICustomizersHolder customizersHolder, PropertyPath propertyPath)
		{
			CustomizersHolder = customizersHolder;
			PropertyPath = propertyPath;
		}

		protected ICustomizersHolder CustomizersHolder { get; private set; }
		protected PropertyPath PropertyPath { get; private set; }

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			Property(property, x => { });
		}

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			RegisterPropertyMapping(property, mapping);
		}

		protected virtual void RegisterPropertyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegistePropertyMapping(mapping, member, memberOf);
		}

		public void Property(string notVidiblePropertyOrFieldName, Action<IPropertyMapper> mapping)
		{
			RegisterNoVisiblePropertyMapping(notVidiblePropertyOrFieldName, mapping);
		}

		protected virtual void RegisterNoVisiblePropertyMapping(string notVidiblePropertyOrFieldName, Action<IPropertyMapper> mapping)
		{
			// even seems repetitive, before unify this registration with the registration using Expression take in account that reflection operations
			// done unsing expressions are faster than those done with pure reflection.
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegistePropertyMapping(mapping, member, memberOf);
		}

		protected void RegistePropertyMapping(Action<IPropertyMapper> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			}
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property,
																			Action<IComponentMapper<TComponent>> mapping) where TComponent : class
		{
			RegisterComponentMapping(property, mapping);
		}
		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property) where TComponent : class
		{
			RegisterComponentMapping(property, x => { });
		}

		protected virtual void RegisterComponentMapping<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
			where TComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterComponentMapping<TComponent>(mapping, member, memberOf);
		}

		protected void RegisterComponentMapping<TComponent>(Action<IComponentMapper<TComponent>> mapping, params MemberInfo[] members)
			where TComponent : class
		{
			foreach (var member in members)
			{
				mapping(new ComponentCustomizer<TComponent>(CustomizersHolder, new PropertyPath(PropertyPath, member)));
			}
		}

		public void Component<TComponent>(Expression<Func<TEntity, IDictionary>> property,
													 TComponent dynamicComponentTemplate,
													 Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			RegisterDynamicComponentMapping(property, mapping);
		}

		protected virtual void RegisterDynamicComponentMapping<TComponent>(Expression<Func<TEntity, IDictionary>> property, Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterDynamicComponentMapping<TComponent>(mapping, member, memberOf);
		}

		protected void RegisterDynamicComponentMapping<TComponent>(Action<IDynamicComponentMapper<TComponent>> mapping, params MemberInfo[] members)
			where TComponent : class
		{
			throw new NotSupportedException();
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			RegisterManyToOneMapping(property, mapping);
		}

		protected virtual void RegisterManyToOneMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterManyToOneMapping<TProperty>(mapping, member, memberOf);
		}

		protected void RegisterManyToOneMapping<TProperty>(Action<IManyToOneMapper> mapping, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			}
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property) where TProperty : class
		{
			ManyToOne(property, x => { });
		}

		public void OneToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IOneToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterOneToOneMapping<TProperty>(mapping, member, memberOf);
		}

		protected void RegisterOneToOneMapping<TProperty>(Action<IOneToOneMapper> mapping, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			}
		}

		public void Any<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			RegisterAnyMapping(property, idTypeOfMetaType, mapping);
		}

		protected virtual void RegisterAnyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterAnyMapping<TProperty>(mapping, idTypeOfMetaType, member, memberOf);
		}

		protected void RegisterAnyMapping<TProperty>(Action<IAnyMapper> mapping, System.Type idTypeOfMetaType, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), (IAnyMapper am) => am.IdType(idTypeOfMetaType));
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
			}
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
															Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterSetMapping(property, collectionMapping, mapping);
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Set(property, collectionMapping, x => { });
		}

		protected virtual void RegisterSetMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterSetMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		protected void RegisterSetMapping<TElement>(Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
															Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterBagMapping(property, collectionMapping, mapping);
		}
		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Bag(property, collectionMapping, x => { });
		}

		protected virtual void RegisterBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		protected void RegisterBagMapping<TElement>(Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															 Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
															 Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterListMapping(property, collectionMapping, mapping);
		}
		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															 Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			List(property, collectionMapping, x => { });
		}

		protected virtual void RegisterListMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterListMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		protected void RegisterListMapping<TElement>(Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(new PropertyPath(null, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
																		Action<IMapKeyRelation<TKey>> keyMapping,
																		Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterMapMapping(property, collectionMapping, keyMapping, mapping);
		}
		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping)
		{
			Map(property, collectionMapping, keyMapping => { }, x => { });
		}

		protected virtual void RegisterMapMapping<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterMapMapping<TKey, TElement>(collectionMapping, keyMapping, mapping, member, memberOf);
		}

		protected virtual void RegisterMapMapping<TKey, TElement>(Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				var memberPath = new PropertyPath(PropertyPath, member);
				collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(memberPath, CustomizersHolder));
				keyMapping(new MapKeyRelationCustomizer<TKey>(memberPath, CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(memberPath, CustomizersHolder));
			}
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
																		Action<ICollectionElementRelation<TElement>> mapping)
		{
			Map(property, collectionMapping, keyMapping => { }, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping,
													Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterIdBagMapping(property, collectionMapping, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			RegisterIdBagMapping(property, collectionMapping, x => { });
		}

		protected virtual void RegisterIdBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterIdBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		protected virtual void RegisterIdBagMapping<TElement>(Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			throw new NotSupportedException();
		}

		public void Set<TElement>(string notVidiblePropertyOrFieldName, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var collectionElementType = member.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (!typeof(TElement).Equals(collectionElementType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a collection of {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterSetMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void Set<TElement>(string notVidiblePropertyOrFieldName, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Set(notVidiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void Bag<TElement>(string notVidiblePropertyOrFieldName, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var collectionElementType = member.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (!typeof(TElement).Equals(collectionElementType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a collection of {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void Bag<TElement>(string notVidiblePropertyOrFieldName, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Bag(notVidiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void List<TElement>(string notVidiblePropertyOrFieldName, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var collectionElementType = member.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (!typeof(TElement).Equals(collectionElementType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a collection of {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterListMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void List<TElement>(string notVidiblePropertyOrFieldName, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			List(notVidiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void Map<TKey, TElement>(string notVidiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			var keyType = propertyOrFieldType.DetermineDictionaryKeyType();
			var collectionElementType = propertyOrFieldType.DetermineDictionaryValueType();
			if (!typeof(TElement).Equals(collectionElementType) || !typeof(TKey).Equals(keyType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a dictionary of {2}/{3} but was {4}/{5}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TKey).Name, keyType.Name, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterMapMapping<TKey, TElement>(collectionMapping, keyMapping, mapping, member, memberOf);
		}

		public void Map<TKey, TElement>(string notVidiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			Map(notVidiblePropertyOrFieldName, collectionMapping, x => { }, mapping);
		}

		public void Map<TKey, TElement>(string notVidiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping)
		{
			Map(notVidiblePropertyOrFieldName, collectionMapping, x => { }, y => { });
		}

		public void IdBag<TElement>(string notVidiblePropertyOrFieldName, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var collectionElementType = member.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (!typeof(TElement).Equals(collectionElementType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a collection of {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterIdBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void IdBag<TElement>(string notVidiblePropertyOrFieldName, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			IdBag(notVidiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void ManyToOne<TProperty>(string notVidiblePropertyOrFieldName, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a many-to-one with {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterManyToOneMapping<TProperty>(mapping, member, memberOf);
		}

		public void Component<TComponent>(string notVidiblePropertyOrFieldName, Action<IComponentMapper<TComponent>> mapping) where TComponent : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TComponent).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a component of {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TComponent).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterComponentMapping<TComponent>(mapping, member, memberOf);
		}

		public void Component<TComponent>(string notVidiblePropertyOrFieldName) where TComponent : class
		{
			Component<TComponent>(notVidiblePropertyOrFieldName, x => { });
		}

		public void Component<TComponent>(string notVidiblePropertyOrFieldName, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterDynamicComponentMapping<TComponent>(mapping, member, memberOf);
		}

		public void Any<TProperty>(string notVidiblePropertyOrFieldName, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a heterogeneous (any) of type {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterAnyMapping<TProperty>(mapping, idTypeOfMetaType, member, memberOf);
		}

		public void OneToOne<TProperty>(string notVidiblePropertyOrFieldName, Action<IOneToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetPropertyOrFieldMatchingNameOrThrow(notVidiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a one-to-one with {2} but was {3}",
																								 notVidiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterOneToOneMapping<TProperty>(mapping, member, memberOf);
		}

		public static MemberInfo GetPropertyOrFieldMatchingNameOrThrow(string memberName)
		{
			var result = typeof(TEntity).GetPropertyOrFieldMatchingName(memberName);
			if (result == null)
			{
				throw new MappingException(string.Format("Member not found. The member '{0}' does not exists in type {1}", memberName, typeof(TEntity).FullName));
			}
			return result;
		}
	}
}