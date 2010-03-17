using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.Patterns;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class Mapper
	{
		internal const BindingFlags PropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		internal const BindingFlags RootClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		private readonly IDomainInspector domainInspector;
		private readonly ICustomizersHolder customizerHolder;
		private readonly IPatternsAppliersHolder patternsAppliers;

		public Mapper(IDomainInspector domainInspector)
			: this(domainInspector, new CustomizersHolder(), new DefaultPatternsAppliersHolder(domainInspector)) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder)
			: this(domainInspector, customizerHolder, new DefaultPatternsAppliersHolder(domainInspector)) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder,
		              IPatternsAppliersHolder patternsAppliers)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if (customizerHolder == null)
			{
				throw new ArgumentNullException("customizerHolder");
			}
			if (patternsAppliers == null)
			{
				throw new ArgumentNullException("patternsAppliers");
			}
			this.domainInspector = domainInspector;
			this.customizerHolder = customizerHolder;
			this.patternsAppliers = patternsAppliers;
		}

		public void Class<TRootEntity>(Action<IClassMapper<TRootEntity>> customizeAction) where TRootEntity : class
		{
			var customizer = new ClassCustomizer<TRootEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void Subclass<TEntity>(Action<ISubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new SubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void JoinedSubclass<TEntity>(Action<IJoinedSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new JoinedSubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void UnionSubclass<TEntity>(Action<IUnionSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new UnionSubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void Customize<TPersistent>(Action<IPersistentClassCustomizer<TPersistent>> customizeAction ) where TPersistent: class
		{
			var customizer = new PersistentClassCustomizer<TPersistent>(customizerHolder);
			customizeAction(customizer);
		}

		public IPatternsAppliersHolder PatternsAppliers
		{
			get { return patternsAppliers; }
		}

		public void AddPoidPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IIdMapper> applier)
		{
			PatternsAppliers.Poid.Add(new DelegatedMemberAdvancedApplier<IIdMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedMemberApplier<IPropertyMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedMemberAdvancedApplier<IPropertyMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedMemberApplier<ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedMemberAdvancedApplier<ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedMemberApplier<IManyToOneMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedMemberAdvancedApplier<IManyToOneMapper>(matcher, applier));
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
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				var classMapper = new SubclassMapper(type, mapping);
				propertiesContainer = classMapper;
				PatternsAppliers.Subclass.ApplyAllMatchs(type, classMapper);
				customizerHolder.InvokeCustomizers(type, classMapper);
			}
			else if (domainInspector.IsTablePerClass(type))
			{
				var classMapper = new JoinedSubclassMapper(type, mapping);
				propertiesContainer = classMapper;
				PatternsAppliers.JoinedSubclass.ApplyAllMatchs(type, classMapper);
				customizerHolder.InvokeCustomizers(type, classMapper);
			}
			else if (domainInspector.IsTablePerConcreteClass(type))
			{
				var classMapper = new UnionSubclassMapper(type, mapping);
				propertiesContainer = classMapper;
				PatternsAppliers.UnionSubclass.ApplyAllMatchs(type, classMapper);
				customizerHolder.InvokeCustomizers(type, classMapper);
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
					PatternsAppliers.Poid.ApplyAllMatchs(poidPropertyOrField, idMapper);
				});
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator();
			}
			PatternsAppliers.RootClass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			MapProperties(type, GetPersistentProperties(type, RootClassPropertiesBindingFlags), classMapper);
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
			MapProperties(propertiesContainerType, GetPersistentProperties(propertiesContainerType, PropertiesBindingFlags), propertiesContainer, null);
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<PropertyInfo> propertiesToMap,
															 IPropertyContainerMapper propertiesContainer)
		{
			MapProperties(propertiesContainerType, propertiesToMap, propertiesContainer, null);			
		}

		private IEnumerable<PropertyInfo> GetPersistentProperties(Type type, BindingFlags propertiesBindingFlags)
		{
			var properties = type.GetProperties(propertiesBindingFlags);
			return properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<PropertyInfo> propertiesToMap,
		                           IPropertyContainerMapper propertiesContainer, PropertyPath path)
		{
			foreach (PropertyInfo property in propertiesToMap)
			{
				MemberInfo member = property;
				Type propertyType = property.GetPropertyOrFieldType();
				var memberPath = new PropertyPath(path, member);
				if(domainInspector.IsComplex(member))
				{
					MapProperty(member, memberPath, propertiesContainer);					
				}
				else if (domainInspector.IsManyToOne(propertiesContainerType, propertyType))
				{
					MapManyToOne(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsOneToOne(propertiesContainerType, propertyType))
				{
					MapOneToOne(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsSet(property))
				{
					MapSet(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsDictionary(property))
				{
					MapDictionary(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsArray(property)) {}
				else if (domainInspector.IsList(property))
				{
					MapList(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsBag(property))
				{
					MapBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsComponent(propertyType))
				{
					MapComponent(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
			}
		}

		private void MapProperty(MemberInfo member, PropertyPath propertyPath, IPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Property(member, propertyMapper =>
				{
					PatternsAppliers.Property.ApplyAllMatchs(member, propertyMapper);
					PatternsAppliers.PropertyPath.ApplyAllMatchs(propertyPath, propertyMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), propertyMapper);
					customizerHolder.InvokeCustomizers(propertyPath, propertyMapper);
				});
		}

		private void MapComponent(MemberInfo member, PropertyPath memberPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.Component(member, x =>
				{
					// Note: should, the Parent relation, be managed through DomainInspector ?
					Type componentType = propertyType;
					IEnumerable<PropertyInfo> persistentProperties = GetPersistentProperties(componentType, PropertiesBindingFlags);
					PropertyInfo parentReferenceProperty =
						persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);
					if (parentReferenceProperty != null)
					{
						x.Parent(parentReferenceProperty);
					}
					MapProperties(propertyType, persistentProperties.Where(pi => pi != parentReferenceProperty), x, memberPath);
				});
		}

		private void MapBag(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, collectionElementType);
			propertiesContainer.Bag(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapList(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                     Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, collectionElementType);
			propertiesContainer.List(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapDictionary(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                           Type propertiesContainerType)
		{
			Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			if (dictionaryKeyType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property{0} in {1})",
				                                              member.Name, propertiesContainerType));
			}
			Type dictionaryValueType = propertyType.DetermineDictionaryValueType();
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, dictionaryValueType);
			propertiesContainer.Map(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapSet(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, collectionElementType);
			propertiesContainer.Set(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), collectionPropertiesMapper);
					customizerHolder.InvokeCustomizers(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapOneToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                         Type propertiesContainerType)
		{
			propertiesContainer.OneToOne(member, oneToOneMapper =>
				{
					Cascade? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					oneToOneMapper.Cascade(cascade.GetValueOrDefault(Cascade.None));
					PatternsAppliers.OneToOne.ApplyAllMatchs(member, oneToOneMapper);
					PatternsAppliers.OneToOnePath.ApplyAllMatchs(propertyPath, oneToOneMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), oneToOneMapper);
					customizerHolder.InvokeCustomizers(propertyPath, oneToOneMapper);
				});
		}

		private void MapManyToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.ManyToOne(member, manyToOneMapper =>
				{
					Cascade? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					manyToOneMapper.Cascade(cascade.GetValueOrDefault(Cascade.None));
					PatternsAppliers.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
					PatternsAppliers.ManyToOnePath.ApplyAllMatchs(propertyPath, manyToOneMapper);
					customizerHolder.InvokeCustomizers(new PropertyPath(null, member), manyToOneMapper);
					customizerHolder.InvokeCustomizers(propertyPath, manyToOneMapper);
				});
		}

		private Type GetCollectionElementTypeOrThrow(Type type, MemberInfo property, Type propertyType)
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
				mapped.Cascade(cascadeToApply.GetValueOrDefault(Cascade.None));
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
				mapped.Cascade(cascadeToApply.GetValueOrDefault(Cascade.None));
			}

			#endregion
		}

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly Type ownerType;
			private readonly Type componentType;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;

			public ComponentRelationMapper(Type ownerType, Type componentType, IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder)
			{
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.domainInspector = domainInspector;
				this.patternsAppliersHolder = patternsAppliersHolder;
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
				// TODO check PropertyPath behaviour when the component is in a collection
				foreach (var property in persistentProperties)
				{
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
							{
								patternsAppliersHolder.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
								customizersHolder.InvokeCustomizers(new PropertyPath(null, member), manyToOneMapper);
							});
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(member, x =>
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
						propertiesContainer.Property(member, propertyMapper =>
							{
								patternsAppliersHolder.Property.ApplyAllMatchs(member, propertyMapper);
								customizersHolder.InvokeCustomizers(new PropertyPath(null, member), propertyMapper);
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
				return new ComponentRelationMapper(ownerType, collectionElementType, domainInspector, PatternsAppliers, customizerHolder);
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