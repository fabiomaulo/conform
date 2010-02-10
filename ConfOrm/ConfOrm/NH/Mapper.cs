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
		internal const BindingFlags PropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		private readonly IDomainInspector domainInspector;

		public Mapper(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public HbmMapping CompileMappingFor(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			string defaultAssemblyName = null;
			string defaultNamespace = null;
			var firstType = types.FirstOrDefault();
			if (firstType != null && types.All(t => t.Assembly.Equals(firstType.Assembly)))
			{
				defaultAssemblyName = firstType.Assembly.GetName().Name;
			}
			if (firstType != null && types.All(t => t.Namespace.Equals(firstType.Namespace)))
			{
				defaultNamespace = firstType.Namespace;
			}
			var mapping = new HbmMapping {assembly = defaultAssemblyName, @namespace = defaultNamespace};
			foreach (var type in RootClasses(types))
			{
				AddRootClassMapping(type, mapping);
			}
			foreach (var type in Subclasses(types))
			{
				AddSubclassMapping(mapping, type);
			}
			return mapping;
		}

		private IEnumerable<Type> Subclasses(IEnumerable<Type> types)
		{
			return types.Where(type => domainInspector.IsEntity(type) && !domainInspector.IsRootEntity(type));
		}

		private IEnumerable<Type> RootClasses(IEnumerable<Type> types)
		{
			return types.Where(type => domainInspector.IsEntity(type) && domainInspector.IsRootEntity(type));
		}

		private void AddSubclassMapping(HbmMapping mapping, Type type)
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

		private void AddRootClassMapping(Type type, HbmMapping mapping)
		{
			var poidPropertyOrField = GetPoidPropertyOrField(type);
			var classMapper = new ClassMapper(type, mapping, poidPropertyOrField);
			var idMapper = new IdMapper(classMapper.Id);
			var persistentIdStrategy = domainInspector.GetPersistentIdStrategy(poidPropertyOrField);
			if(persistentIdStrategy != null)
			{
				idMapper.Generator(GetGenerator(persistentIdStrategy.Strategy), gm =>
					{
						if(persistentIdStrategy.Params != null)
						{
							gm.Params(persistentIdStrategy.Params);
						}
					});
			}
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator();
			}
			MapProperties(type, classMapper);
		}

		private Generators GetGenerator(PoIdStrategy strategy)
		{
			switch (strategy)
			{
				case PoIdStrategy.HighLow:
					return Generators.HighLow;
				case PoIdStrategy.Sequence:
					return Generators.Native;
				case PoIdStrategy.Guid:
					return Generators.Native;
				case PoIdStrategy.GuidOptimized:
					return Generators.Native;
				case PoIdStrategy.Identity:
					return Generators.Native;
				case PoIdStrategy.Native:
					return Generators.Native;
				default:
					throw new ArgumentOutOfRangeException("strategy");
			}
		}

		private void MapProperties(Type propertiesContainerType, IPropertyContainerMapper propertiesContainer)
		{
			MapProperties(propertiesContainerType, GetPersistentProperties(propertiesContainerType), propertiesContainer);
		}

		private IEnumerable<PropertyInfo> GetPersistentProperties(Type type)
		{
			var properties = type.GetProperties(PropertiesBindingFlags);
			return properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<PropertyInfo> propertiesToMap, IPropertyContainerMapper propertiesContainer)
		{
			foreach (var property in propertiesToMap)
			{
				MemberInfo member = property;
				var propertyType = property.GetPropertyOrFieldType();
				if(domainInspector.IsManyToOne(propertiesContainerType, propertyType))
				{
					propertiesContainer.ManyToOne(property, x =>
						{
							var cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
							if(cascade != Cascade.None)
							{
								x.Cascade(cascade);
							}
						});
				}
				else if (domainInspector.IsOneToOne(propertiesContainerType, propertyType))
				{
					propertiesContainer.OneToOne(property, x =>
						{
							var cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
							if (cascade != Cascade.None)
							{
								x.Cascade(cascade);
							}
						});
				}
				else if (domainInspector.IsSet(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Set(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsDictionary(property))
				{
					Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
					if (dictionaryKeyType == null)
					{
						throw new NotSupportedException(string.Format("Can't determine collection element relation (property{0} in {1})",
						                                              property.Name, propertiesContainerType));
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
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.List(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsBag(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Bag(property, cert.MapCollectionProperties, cert.Map);
				}
				else if (domainInspector.IsComponent(propertyType))
				{
					propertiesContainer.Component(property, x =>
						{
							// Note: should, the Parent relation, be managed through DomainInspector ?
							var componentType = propertyType;
							var persistentProperties = GetPersistentProperties(componentType);
							var parentReferenceProperty =
								persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);
							if (parentReferenceProperty != null)
							{
								x.Parent(parentReferenceProperty);
							}
							MapProperties(propertyType, persistentProperties.Where(pi => pi != parentReferenceProperty), x);
						});
				}
				else
				{
					propertiesContainer.Property(member, x =>
						{
							var propertyInfo = member as PropertyInfo;
							if (propertyInfo != null)
							{
								Accessor accessor = GetAccessor(propertyInfo);
								if (accessor != Accessor.Property)
								{
									x.Access(accessor);
								}
							}
						});
				}
			}
		}

		private Accessor GetAccessor(PropertyInfo property)
		{
			var persistentPropertyAccessStrategy = domainInspector.PersistentPropertyAccessStrategy(property);
			if(persistentPropertyAccessStrategy != StateAccessStrategy.Property)
			{
				switch (persistentPropertyAccessStrategy)
				{
					case StateAccessStrategy.Field:
						return Accessor.Field;
					case StateAccessStrategy.FieldOnSet:
						return Accessor.NoSetter;
					case StateAccessStrategy.ReadOnlyProperty:
						return Accessor.ReadOnly;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (!property.CanWrite)
			{
				return Accessor.NoSetter;
			}
			return Accessor.Property;
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
			private readonly MemberInfo member;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;

			public OneToManyRelationMapper(MemberInfo member, Type ownerType, Type collectionElementType, IDomainInspector domainInspector)
			{
				this.member = member;
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
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				if(cascadeToApply != Cascade.None)
				{
					mapped.Cascade(cascadeToApply);
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
			private readonly MemberInfo member;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;

			public ManyToManyRelationMapper(MemberInfo member, Type ownerType, Type collectionElementType, IDomainInspector domainInspector)
			{
				this.member = member;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToMany();
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				if (cascadeToApply != Cascade.None)
				{
					mapped.Cascade(cascadeToApply);
				}
			}

			#endregion
		}

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly Type ownerType;
			private readonly Type componentType;
			private readonly IDomainInspector domainInspector;

			public ComponentRelationMapper(Type ownerType, Type componentType, IDomainInspector domainInspector)
			{
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.domainInspector = domainInspector;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Component(x =>
					{
						// Note: should, the Parent relation, be managed through DomainInspector ?
						var persistentProperties = GetPersistentProperties(componentType);
						var parentReferenceProperty =
							persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == ownerType);
						if (parentReferenceProperty != null)
						{
							x.Parent(parentReferenceProperty);
						}
						MapProperties(componentType, x, persistentProperties.Where(pi => pi != parentReferenceProperty));
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion

			private IEnumerable<PropertyInfo> GetPersistentProperties(Type type)
			{
				var properties = type.GetProperties(PropertiesBindingFlags);
				return properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));
			}

			private void MapProperties(Type type, IComponentElementMapper propertiesContainer, IEnumerable<PropertyInfo> persistentProperties)
			{
				foreach (var property in persistentProperties)
				{
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(property);
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(property, x =>
							{
								// Note: for nested-components the Parent discovering is mandatory (recursive nested-component)
								var componetOwnerType = type;
								var componetPropertyType = propertyType;

								var componentProperties = GetPersistentProperties(componetPropertyType);
								var parentReferenceProperty = componentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == componetOwnerType);
								if (parentReferenceProperty != null)
								{
									x.Parent(parentReferenceProperty);
								}
								MapProperties(componetPropertyType, x, componentProperties.Where(pi => pi != parentReferenceProperty));
							});
					}
					else
					{
						propertiesContainer.Property(property, x => { });
					}
				}
			}
		}

		protected virtual ICollectionElementRelationMapper DetermineCollectionElementRelationType(MemberInfo property, Type collectionElementType)
		{
			var ownerType = property.DeclaringType;
			if (domainInspector.IsOneToMany(ownerType, collectionElementType))
			{
				return new OneToManyRelationMapper(property, ownerType, collectionElementType, domainInspector);
			}
			else if (domainInspector.IsManyToMany(ownerType, collectionElementType))
			{
				return new ManyToManyRelationMapper(property, ownerType, collectionElementType, domainInspector);
			}
			else if (domainInspector.IsComponent(collectionElementType))
			{
				return new ComponentRelationMapper(ownerType, collectionElementType, domainInspector);
			}
			return new ElementRelationMapper();
		}

		private MemberInfo GetPoidPropertyOrField(Type type)
		{
			return type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi=> domainInspector.IsPersistentId(mi));
		}

		public IEnumerable<HbmMapping> CompileMappingForEach(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			foreach (var type in RootClasses(types))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				AddRootClassMapping(type, mapping);
				yield return mapping;
			}
			foreach (var type in Subclasses(types))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				AddSubclassMapping(mapping, type);
				yield return mapping;
			}
		}
	}
}