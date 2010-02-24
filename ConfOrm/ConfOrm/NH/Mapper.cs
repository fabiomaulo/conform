using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.Patterns;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class Mapper : ICustomizersHolder
	{
		internal const BindingFlags PropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		private readonly IDomainInspector domainInspector;
		private readonly List<IPatternApplier<MemberInfo, IPropertyMapper>> propertyPatternsAppliers;
		private readonly List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collectionPatternsAppliers;

		private readonly Dictionary<MemberInfo, List<Action<IPropertyMapper>>> propertyCustomizers = new Dictionary<MemberInfo, List<Action<IPropertyMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<IManyToOneMapper>>> manyToOneCustomizers = new Dictionary<MemberInfo, List<Action<IManyToOneMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<IOneToOneMapper>>> oneToOneCustomizers = new Dictionary<MemberInfo, List<Action<IOneToOneMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<ISetPropertiesMapper>>> setCustomizers = new Dictionary<MemberInfo, List<Action<ISetPropertiesMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<IBagPropertiesMapper>>> bagCustomizers = new Dictionary<MemberInfo, List<Action<IBagPropertiesMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<IListPropertiesMapper>>> listCustomizers = new Dictionary<MemberInfo, List<Action<IListPropertiesMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<IMapPropertiesMapper>>> mapCustomizers = new Dictionary<MemberInfo, List<Action<IMapPropertiesMapper>>>();
		private readonly Dictionary<MemberInfo, List<Action<ICollectionPropertiesMapper>>> collectionCustomizers = new Dictionary<MemberInfo, List<Action<ICollectionPropertiesMapper>>>();

		private readonly Dictionary<Type, List<Action<IClassAttributesMapper>>> rootClassCustomizers =
			new Dictionary<Type, List<Action<IClassAttributesMapper>>>();

		private readonly Dictionary<Type, List<Action<ISubclassAttributesMapper>>> subclassCustomizers =
			new Dictionary<Type, List<Action<ISubclassAttributesMapper>>>();

		private readonly Dictionary<Type, List<Action<IJoinedSubclassAttributesMapper>>> joinedClassCustomizers =
			new Dictionary<Type, List<Action<IJoinedSubclassAttributesMapper>>>();

		private readonly Dictionary<Type, List<Action<IUnionSubclassAttributesMapper>>> unionClassCustomizers =
			new Dictionary<Type, List<Action<IUnionSubclassAttributesMapper>>>();

		private readonly Dictionary<Type, List<Action<IComponentMapper>>> componetClassCustomizers =
			new Dictionary<Type, List<Action<IComponentMapper>>>();

		public Mapper(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;

			propertyPatternsAppliers = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			                           	{
			                           		new ReadOnlyPropertyAccessorApplier(),
			                           		new NoSetterPropertyToFieldAccessorApplier(),
			                           		new PropertyToFieldAccessorApplier()
			                           	};
			collectionPatternsAppliers = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			                             	{
			                             		new ReadOnlyCollectionPropertyAccessorApplier(),
			                             		new NoSetterCollectionPropertyToFieldAccessorApplier(),
			                             		new CollectionPropertyToFieldAccessorApplier(),
			                             		new BidirectionalOneToManyApplier(domainInspector),
			                             		new BidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
			                             	};
		}

		public void Class<TRootEntity>(Action<IClassMapper<TRootEntity>> customizeAction) where TRootEntity : class
		{
			var customizer = new ClassCustomizer<TRootEntity>(this);
			customizeAction(customizer);
		}

		public void Customize<TPersistent>(Action<IPersistentClassCustomizer<TPersistent>> customizeAction ) where TPersistent: class
		{
			var customizer = new PersistentClassCustomizer<TPersistent>(this);
			customizeAction(customizer);
		}

		public ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> PropertyPatternsAppliers
		{
			get { return propertyPatternsAppliers; }
		}

		public List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> CollectionPatternsAppliers
		{
			get { return collectionPatternsAppliers; }
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<IPropertyMapper> applier)
		{
			propertyPatternsAppliers.Add(new DelegatedPropertyApplier(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<ICollectionPropertiesMapper> applier)
		{
			collectionPatternsAppliers.Add(new DelegatedCollectionApplier(matcher, applier));
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
			classMapper.Id(idMapper =>
				{
					var persistentIdStrategy = domainInspector.GetPersistentIdStrategy(poidPropertyOrField);
					if (persistentIdStrategy != null && persistentIdStrategy.Strategy != PoIdStrategy.Assigned)
					{
						idMapper.Generator(GetGenerator(persistentIdStrategy.Strategy), gm =>
						{
							if (persistentIdStrategy.Params != null)
							{
								gm.Params(persistentIdStrategy.Params);
							}
						});
					}
				});
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator();
			}
			InvokeCustomizers(type, classMapper);
			MapProperties(type, classMapper);
		}

		private IGeneratorDef GetGenerator(PoIdStrategy strategy)
		{
			switch (strategy)
			{
				case PoIdStrategy.HighLow:
					return Generators.HighLow;
				case PoIdStrategy.Sequence:
					return Generators.Sequence;
				case PoIdStrategy.Guid:
					return Generators.Guid;
				case PoIdStrategy.GuidOptimized:
					return Generators.GuidComb;
				case PoIdStrategy.Identity:
					return Generators.Identity;
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
					propertiesContainer.ManyToOne(property, manyToOneMapper =>
						{
							var cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
							if(cascade != Cascade.None)
							{
								manyToOneMapper.Cascade(cascade);
							}
							InvokeCustomizers(member, manyToOneMapper);
						});
				}
				else if (domainInspector.IsOneToOne(propertiesContainerType, propertyType))
				{
					propertiesContainer.OneToOne(property, oneToOneMapper =>
						{
							var cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
							if (cascade != Cascade.None)
							{
								oneToOneMapper.Cascade(cascade);
							}
							InvokeCustomizers(member, oneToOneMapper);
						});
				}
				else if (domainInspector.IsSet(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Set(property, collectionPropertiesMapper=>
						{
							cert.MapCollectionProperties(collectionPropertiesMapper);
							collectionPatternsAppliers.ApplyAllMatchs(member, collectionPropertiesMapper);
							InvokeCustomizers(member, collectionPropertiesMapper);
						}, cert.Map);
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
					var cert = DetermineCollectionElementRelationType(property, dictionaryValueType);
					propertiesContainer.Map(property, collectionPropertiesMapper =>
						{
							cert.MapCollectionProperties(collectionPropertiesMapper);
							collectionPatternsAppliers.ApplyAllMatchs(member, collectionPropertiesMapper);
							InvokeCustomizers(member, collectionPropertiesMapper);
						}, cert.Map);
				}
				else if (domainInspector.IsArray(property))
				{
				}
				else if (domainInspector.IsList(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.List(property, collectionPropertiesMapper =>
					{
						cert.MapCollectionProperties(collectionPropertiesMapper);
						collectionPatternsAppliers.ApplyAllMatchs(member, collectionPropertiesMapper);
						InvokeCustomizers(member, collectionPropertiesMapper);
					}, cert.Map);
				}
				else if (domainInspector.IsBag(property))
				{
					Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, property, propertyType);
					var cert = DetermineCollectionElementRelationType(property, collectionElementType);
					propertiesContainer.Bag(property, collectionPropertiesMapper =>
					{
						cert.MapCollectionProperties(collectionPropertiesMapper);
						collectionPatternsAppliers.ApplyAllMatchs(member, collectionPropertiesMapper);
						InvokeCustomizers(member, collectionPropertiesMapper);
					}, cert.Map);
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
					propertiesContainer.Property(member, propertyMapper =>
						{
							propertyPatternsAppliers.ApplyAllMatchs(member, propertyMapper);
							InvokeCustomizers(member, propertyMapper);
						});
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
			private readonly ICustomizersHolder customizersHolder;

			public ComponentRelationMapper(Type ownerType, Type componentType, IDomainInspector domainInspector, ICustomizersHolder customizersHolder)
			{
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.domainInspector = domainInspector;
				this.customizersHolder = customizersHolder;
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
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(property, manyToOneMapper =>
							{
								customizersHolder.InvokeCustomizers(member, manyToOneMapper);
							});
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
						propertiesContainer.Property(property, propertyMapper =>
							{
								customizersHolder.InvokeCustomizers(member, propertyMapper);
							});
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
				return new ComponentRelationMapper(ownerType, collectionElementType, domainInspector, this);
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

		#region Implementation of ICustomizersHolder

		public void AddCustomizer(Type type, Action<IClassAttributesMapper> classCustomizer)
		{
			AddCustomizer(rootClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<ISubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(subclassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IJoinedSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(joinedClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IUnionSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(unionClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IComponentMapper> classCustomizer)
		{
			AddCustomizer(componetClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IPropertyMapper> propertyCustomizer)
		{
			AddCustomizer(propertyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IManyToOneMapper> propertyCustomizer)
		{
			AddCustomizer(manyToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IOneToOneMapper> propertyCustomizer)
		{
			AddCustomizer(oneToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<ISetPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(setCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IBagPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(bagCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IListPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(listCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IMapPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(mapCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<ICollectionPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(collectionCustomizers, member, propertyCustomizer);
		}

		public void InvokeCustomizers(Type type, IClassAttributesMapper mapper)
		{
			InvokeCustomizers(rootClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, ISubclassAttributesMapper mapper)
		{
			InvokeCustomizers(subclassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IJoinedSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(joinedClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IUnionSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(unionClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IComponentMapper mapper)
		{
			InvokeCustomizers(componetClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IPropertyMapper mapper)
		{
			InvokeCustomizers(propertyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IManyToOneMapper mapper)
		{
			InvokeCustomizers(manyToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IOneToOneMapper mapper)
		{
			InvokeCustomizers(oneToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, ISetPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(setCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IBagPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(bagCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IListPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(listCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IMapPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(mapCustomizers, member, mapper);
		}

		private void AddCustomizer<TSubject, TCustomizable>(IDictionary<TSubject, List<Action<TCustomizable>>> customizers, TSubject member, Action<TCustomizable> customizer)
		{
			List<Action<TCustomizable>> actions;
			if (!customizers.TryGetValue(member, out actions))
			{
				actions = new List<Action<TCustomizable>>();
				customizers[member] = actions;
			}
			actions.Add(customizer);
		}

		private void InvokeCustomizers<TSubject, TCustomizable>(IDictionary<TSubject, List<Action<TCustomizable>>> customizers, TSubject member, TCustomizable customizable)
		{
			List<Action<TCustomizable>> actions;
			if (customizers.TryGetValue(member, out actions))
			{
				foreach (var action in actions)
				{
					action(customizable);
				}
			}
		}

		#endregion
	}
}