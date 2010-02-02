using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class Mapper
	{
		private readonly HbmMapping mapping;
		private readonly IDomainInspector domainInspector;

		public Mapper(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
			mapping = new HbmMapping();
		}

		public HbmMapping CompileMappingFor(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			// Map root classes
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && domainInspector.IsRootEntity(type)))
			{
				var classMapper = new ClassMapper(type, mapping, GetPoidPropertyOrField(type));
				new IdMapper(classMapper.Id);
				if (domainInspector.IsTablePerClassHierarchy(type))
				{
					classMapper.Discriminator();
				}
				MapProperties(type, classMapper);
			}
			// Map subclasses
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && !domainInspector.IsRootEntity(type)))
			{
				IPropertyContainerMapper propertiesContainer = null;
				if (domainInspector.IsTablePerClass(type))
				{
					var classMapper = new JoinedSubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				else if (domainInspector.IsTablePerClassHierarchy(type))
				{
					var classMapper = new SubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				else if (domainInspector.IsTablePerConcreteClass(type))
				{
					var classMapper = new UnionSubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				MapProperties(type, propertiesContainer);
			}
			return mapping;
		}

		private void MapProperties(Type type, IPropertyContainerMapper propertiesContainer)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (var property in properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)))
			{
				var propertyType = property.GetPropertyOrFieldType();
				if(domainInspector.IsManyToOne(type, propertyType))
				{
					propertiesContainer.ManyToOne(property);
				}
				else if (domainInspector.IsOneToOne(type, propertyType))
				{
					propertiesContainer.OneToOne(property, x => { });
				}
				else if (domainInspector.IsSet(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(type, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Set(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsDictionary(property))
				{
					Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
					if (dictionaryKeyType == null)
					{
						throw new NotSupportedException(string.Format("Can't determine collection element relation (property{0} in {1})",
																													property.Name, type));
					}
					Type dictionaryValueType = propertyType.DetermineDictionaryValueType();
					// TODO : determine RelationType for Key
					var cert = DetermineCollectionElementRelationType(property, dictionaryValueType);
					propertiesContainer.Map(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsArray(property))
				{
				}
				else if (domainInspector.IsList(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(type, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.List(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsBag(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(type, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Bag(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsComponent(propertyType))
				{
					propertiesContainer.Component(property, x => MapProperties(propertyType, x));
				}
				else
				{
					propertiesContainer.Property(property);
				}
			}
		}

		private Type GetCollectionElementTypeOrThrow(Type type, PropertyInfo property, Type propertyType)
		{
			Type collectionElementType = propertyType.DetermineCollectionElementType();
			if (collectionElementType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property{0} in {1})",
				                                              property.Name, type));
			}
			return collectionElementType;
		}

		protected interface ICollectionElementRelationMapper
		{
			void Map(ICollectionElementRelation relation);
			void MapCollectionProperties(ICollectionPropertiesMapper mapped);
		}

		private class ElementRelationMapper : ICollectionElementRelationMapper
		{
			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Element();
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion
		}

		private class OneToManyRelationMapper : ICollectionElementRelationMapper
		{
			private const BindingFlags FlattenHierarchyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;

			public OneToManyRelationMapper(Type ownerType, Type collectionElementType, IDomainInspector domainInspector)
			{
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.OneToMany();
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var parentColumnNameInChild = GetParentColumnNameInChild();
				if (parentColumnNameInChild != null)
				{
					mapped.Inverse = true;
					mapped.Key(k => k.Column(parentColumnNameInChild));
				}
			}

			private string GetParentColumnNameInChild()
			{
				var propertyInfo = collectionElementType.GetProperties(FlattenHierarchyBindingFlags).FirstOrDefault(p => p.PropertyType.IsAssignableFrom(ownerType));
				if (propertyInfo != null)
				{
					return propertyInfo.Name;
				}
				return null;
			}

			#endregion
		}

		private class ManyToManyRelationMapper : ICollectionElementRelationMapper
		{
			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToMany();
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion
		}

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly Type componentType;
			private readonly IDomainInspector domainInspector;

			public ComponentRelationMapper(Type componentType, IDomainInspector domainInspector)
			{
				this.componentType = componentType;
				this.domainInspector = domainInspector;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Component(x => MapProperties(componentType, x));
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion

			private void MapProperties(Type type, IComponentElementMapper propertiesContainer)
			{
				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				foreach (var property in properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)))
				{
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(property);
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(property, x => MapProperties(propertyType, x));
					}
					else
					{
						propertiesContainer.Property(property);
					}
				}
			}
		}

		protected virtual ICollectionElementRelationMapper DetermineCollectionElementRelationType(MemberInfo property, Type collectionElementType)
		{
			var ownerType = property.DeclaringType;
			if (domainInspector.IsOneToMany(ownerType, collectionElementType))
			{
				return new OneToManyRelationMapper(ownerType, collectionElementType, domainInspector);
			}
			else if (domainInspector.IsManyToMany(ownerType, collectionElementType))
			{
				return new ManyToManyRelationMapper();
			}
			else if (domainInspector.IsComponent(collectionElementType))
			{
				return new ComponentRelationMapper(collectionElementType, domainInspector);
			}
			return new ElementRelationMapper();
		}

		private MemberInfo GetPoidPropertyOrField(Type type)
		{
			return type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi=> domainInspector.IsPersistentId(mi));
		}
	}
}