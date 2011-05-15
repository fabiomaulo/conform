using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH.CustomizersImpl;
using ConfOrm.Patterns;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class Mapper
	{
		private readonly IDomainInspector domainInspector;
		private readonly ICustomizersHolder customizerHolder;
		private readonly IPatternsAppliersHolder patternsAppliers;
		private readonly ICandidatePersistentMembersProvider membersProvider;

		public Mapper(IDomainInspector domainInspector)
			: this(
				domainInspector, new CustomizersHolder(), new DefaultPatternsAppliersHolder(domainInspector),
				new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder)
			: this(
				domainInspector, customizerHolder, new DefaultPatternsAppliersHolder(domainInspector),
				new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliers)
			: this(domainInspector, new CustomizersHolder(), patternsAppliers, new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder,
		              IPatternsAppliersHolder patternsAppliers, ICandidatePersistentMembersProvider membersProvider)
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
			if (membersProvider == null)
			{
				throw new ArgumentNullException("membersProvider");
			}
			this.domainInspector = domainInspector;
			this.customizerHolder = customizerHolder;
			this.patternsAppliers = patternsAppliers;
			this.membersProvider = membersProvider;
		}

		#region Events

		/// <summary>
		/// Occurs before apply pattern-appliers on a root class.
		/// </summary>
		public event RootClassMappingHandler BeforeMapClass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a subclass.
		/// </summary>
		public event SubclassMappingHandler BeforeMapSubclass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a joined-subclass.
		/// </summary>
		public event JoinedSubclassMappingHandler BeforeMapJoinedSubclass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a union-subclass.
		/// </summary>
		public event UnionSubclassMappingHandler BeforeMapUnionSubclass;

		public event PropertyMappingHandler BeforeMapProperty;

		public event ManyToOneMappingHandler BeforeMapManyToOne;

		public event OneToOneMappingHandler BeforeMapOneToOne;

		public event AnyMappingHandler BeforeMapAny;

		public event ComponentMappingHandler BeforeMapComponent;

		public event SetMappingHandler BeforeMapSet;

		public event BagMappingHandler BeforeMapBag;

		public event ListMappingHandler BeforeMapList;

		public event MapMappingHandler BeforeMapMap;

		public event ManyToManyMappingHandler BeforeMapManyToMany;

		public event ElementMappingHandler BeforeMapElement;

		public event OneToManyMappingHandler BeforeMapOneToMany;

		public event MapKeyManyToManyMappingHandler BeforeMapMapKeyManyToMany;
		public event MapKeyMappingHandler BeforeMapMapKey;

		/// <summary>
		/// Occurs after apply the last customizer on a root class.
		/// </summary>
		public event RootClassMappingHandler AfterMapClass;

		/// <summary>
		/// Occurs after apply the last customizer on a subclass.
		/// </summary>
		public event SubclassMappingHandler AfterMapSubclass;

		/// <summary>
		/// Occurs after apply the last customizer on a joined-subclass..
		/// </summary>
		public event JoinedSubclassMappingHandler AfterMapJoinedSubclass;

		/// <summary>
		/// Occurs after apply the last customizer on a union-subclass..
		/// </summary>
		public event UnionSubclassMappingHandler AfterMapUnionSubclass;

		public event PropertyMappingHandler AfterMapProperty;

		public event ManyToOneMappingHandler AfterMapManyToOne;

		public event OneToOneMappingHandler AfterMapOneToOne;

		public event AnyMappingHandler AfterMapAny;

		public event ComponentMappingHandler AfterMapComponent;

		public event SetMappingHandler AfterMapSet;

		public event BagMappingHandler AfterMapBag;

		public event ListMappingHandler AfterMapList;

		public event MapMappingHandler AfterMapMap;

		public event ManyToManyMappingHandler AfterMapManyToMany;

		public event ElementMappingHandler AfterMapElement;

		public event OneToManyMappingHandler AfterMapOneToMany;

		public event MapKeyManyToManyMappingHandler AfterMapMapKeyManyToMany;

		public event MapKeyMappingHandler AfterMapMapKey;

		private void InvokeBeforeMapUnionSubclass(Type type, IUnionSubclassAttributesMapper unionsubclasscustomizer)
		{
			UnionSubclassMappingHandler handler = BeforeMapUnionSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, unionsubclasscustomizer);
			}
		}

		private void InvokeBeforeMapJoinedSubclass(Type type, IJoinedSubclassAttributesMapper joinedsubclasscustomizer)
		{
			JoinedSubclassMappingHandler handler = BeforeMapJoinedSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, joinedsubclasscustomizer);
			}
		}

		private void InvokeBeforeMapSubclass(Type type, ISubclassAttributesMapper subclasscustomizer)
		{
			SubclassMappingHandler handler = BeforeMapSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, subclasscustomizer);
			}
		}

		private void InvokeBeforeMapClass(Type type, IClassAttributesMapper classcustomizer)
		{
			RootClassMappingHandler handler = BeforeMapClass;
			if (handler != null)
			{
				handler(DomainInspector, type, classcustomizer);
			}
		}

		private void InvokeBeforeMapProperty(PropertyPath member, IPropertyMapper propertycustomizer)
		{
			PropertyMappingHandler handler = BeforeMapProperty;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapManyToOne(PropertyPath member, IManyToOneMapper propertycustomizer)
		{
			ManyToOneMappingHandler handler = BeforeMapManyToOne;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapOneToOne(PropertyPath member, IOneToOneMapper propertycustomizer)
		{
			OneToOneMappingHandler handler = BeforeMapOneToOne;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapAny(PropertyPath member, IAnyMapper propertycustomizer)
		{
			AnyMappingHandler handler = BeforeMapAny;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapComponent(PropertyPath member, IComponentAttributesMapper propertycustomizer)
		{
			ComponentMappingHandler handler = BeforeMapComponent;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapSet(PropertyPath member, ISetPropertiesMapper propertycustomizer)
		{
			SetMappingHandler handler = BeforeMapSet;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapBag(PropertyPath member, IBagPropertiesMapper propertycustomizer)
		{
			BagMappingHandler handler = BeforeMapBag;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapList(PropertyPath member, IListPropertiesMapper propertycustomizer)
		{
			ListMappingHandler handler = BeforeMapList;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapMap(PropertyPath member, IMapPropertiesMapper propertycustomizer)
		{
			MapMappingHandler handler = BeforeMapMap;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapManyToMany(PropertyPath member, IManyToManyMapper collectionrelationmanytomanycustomizer)
		{
			ManyToManyMappingHandler handler = BeforeMapManyToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationmanytomanycustomizer);
			}
		}

		private void InvokeBeforeMapElement(PropertyPath member, IElementMapper collectionrelationelementcustomizer)
		{
			ElementMappingHandler handler = BeforeMapElement;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationelementcustomizer);
			}
		}

		private void InvokeBeforeMapOneToMany(PropertyPath member, IOneToManyMapper collectionrelationonetomanycustomizer)
		{
			OneToManyMappingHandler handler = BeforeMapOneToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationonetomanycustomizer);
			}
		}

		private void InvokeBeforeMapMapKeyManyToMany(PropertyPath member, IMapKeyManyToManyMapper mapkeymanytomanycustomizer)
		{
			MapKeyManyToManyMappingHandler handler = BeforeMapMapKeyManyToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, mapkeymanytomanycustomizer);
			}
		}

		private void InvokeBeforeMapMapKey(PropertyPath member, IMapKeyMapper mapkeyelementcustomizer)
		{
			MapKeyMappingHandler handler = BeforeMapMapKey;
			if (handler != null)
			{
				handler(DomainInspector, member, mapkeyelementcustomizer);
			}
		}

		private void InvokeAfterMapUnionSubclass(Type type, IUnionSubclassAttributesMapper unionsubclasscustomizer)
		{
			UnionSubclassMappingHandler handler = AfterMapUnionSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, unionsubclasscustomizer);
			}
		}

		private void InvokeAfterMapJoinedSubclass(Type type, IJoinedSubclassAttributesMapper joinedsubclasscustomizer)
		{
			JoinedSubclassMappingHandler handler = AfterMapJoinedSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, joinedsubclasscustomizer);
			}
		}

		private void InvokeAfterMapSubclass(Type type, ISubclassAttributesMapper subclasscustomizer)
		{
			SubclassMappingHandler handler = AfterMapSubclass;
			if (handler != null)
			{
				handler(DomainInspector, type, subclasscustomizer);
			}
		}

		private void InvokeAfterMapClass(Type type, IClassAttributesMapper classcustomizer)
		{
			RootClassMappingHandler handler = AfterMapClass;
			if (handler != null)
			{
				handler(DomainInspector, type, classcustomizer);
			}
		}

		private void InvokeAfterMapProperty(PropertyPath member, IPropertyMapper propertycustomizer)
		{
			PropertyMappingHandler handler = AfterMapProperty;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapManyToOne(PropertyPath member, IManyToOneMapper propertycustomizer)
		{
			ManyToOneMappingHandler handler = AfterMapManyToOne;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapOneToOne(PropertyPath member, IOneToOneMapper propertycustomizer)
		{
			OneToOneMappingHandler handler = AfterMapOneToOne;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapAny(PropertyPath member, IAnyMapper propertycustomizer)
		{
			AnyMappingHandler handler = AfterMapAny;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapComponent(PropertyPath member, IComponentAttributesMapper propertycustomizer)
		{
			ComponentMappingHandler handler = AfterMapComponent;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapSet(PropertyPath member, ISetPropertiesMapper propertycustomizer)
		{
			SetMappingHandler handler = AfterMapSet;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapBag(PropertyPath member, IBagPropertiesMapper propertycustomizer)
		{
			BagMappingHandler handler = AfterMapBag;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapList(PropertyPath member, IListPropertiesMapper propertycustomizer)
		{
			ListMappingHandler handler = AfterMapList;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapMap(PropertyPath member, IMapPropertiesMapper propertycustomizer)
		{
			MapMappingHandler handler = AfterMapMap;
			if (handler != null)
			{
				handler(DomainInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapManyToMany(PropertyPath member, IManyToManyMapper collectionrelationmanytomanycustomizer)
		{
			ManyToManyMappingHandler handler = AfterMapManyToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationmanytomanycustomizer);
			}
		}

		private void InvokeAfterMapElement(PropertyPath member, IElementMapper collectionrelationelementcustomizer)
		{
			ElementMappingHandler handler = AfterMapElement;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationelementcustomizer);
			}
		}

		private void InvokeAfterMapOneToMany(PropertyPath member, IOneToManyMapper collectionrelationonetomanycustomizer)
		{
			OneToManyMappingHandler handler = AfterMapOneToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, collectionrelationonetomanycustomizer);
			}
		}

		private void InvokeAfterMapMapKeyManyToMany(PropertyPath member, IMapKeyManyToManyMapper mapkeymanytomanycustomizer)
		{
			MapKeyManyToManyMappingHandler handler = AfterMapMapKeyManyToMany;
			if (handler != null)
			{
				handler(DomainInspector, member, mapkeymanytomanycustomizer);
			}
		}

		private void InvokeAfterMapMapKey(PropertyPath member, IMapKeyMapper mapkeyelementcustomizer)
		{
			MapKeyMappingHandler handler = AfterMapMapKey;
			if (handler != null)
			{
				handler(DomainInspector, member, mapkeyelementcustomizer);
			}
		}

		#endregion

		public IDomainInspector DomainInspector
		{
			get { return domainInspector; }
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

		public void Component<TComponent>(Action<IComponentMapper<TComponent>> customizeAction) where TComponent : class
		{
			var customizer = new ComponentCustomizer<TComponent>(customizerHolder);
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
			PatternsAppliers.Poid.Add(new DelegatedAdvancedApplier<MemberInfo, IIdMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedApplier<MemberInfo, IPropertyMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedApplier<MemberInfo, ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedAdvancedApplier<MemberInfo, ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedApplier<MemberInfo, IManyToOneMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedAdvancedApplier<MemberInfo, IManyToOneMapper>(matcher, applier));
		}

		public void AddRootClassPattern(Predicate<Type> matcher, Action<Type, IClassAttributesMapper> applier)
		{
			PatternsAppliers.RootClass.Add(new DelegatedAdvancedApplier<Type, IClassAttributesMapper>(matcher, applier));
		}

		public void AddSubclassPattern(Predicate<Type> matcher, Action<Type, ISubclassAttributesMapper> applier)
		{
			PatternsAppliers.Subclass.Add(new DelegatedAdvancedApplier<Type, ISubclassAttributesMapper>(matcher, applier));
		}

		public void AddJoinedSubclassPattern(Predicate<Type> matcher, Action<Type, IJoinedSubclassAttributesMapper> applier)
		{
			PatternsAppliers.JoinedSubclass.Add(new DelegatedAdvancedApplier<Type, IJoinedSubclassAttributesMapper>(matcher, applier));
		}

		public void AddUnionSubclassPattern(Predicate<Type> matcher, Action<Type, IUnionSubclassAttributesMapper> applier)
		{
			PatternsAppliers.UnionSubclass.Add(new DelegatedAdvancedApplier<Type, IUnionSubclassAttributesMapper>(matcher, applier));
		}

		public void AddRootClassPattern(Predicate<Type> matcher, Action<IClassAttributesMapper> applier)
		{
			PatternsAppliers.RootClass.Add(new DelegatedApplier<Type, IClassAttributesMapper>(matcher, applier));
		}

		public void AddSubclassPattern(Predicate<Type> matcher, Action<ISubclassAttributesMapper> applier)
		{
			PatternsAppliers.Subclass.Add(new DelegatedApplier<Type, ISubclassAttributesMapper>(matcher, applier));
		}

		public void AddJoinedSubclassPattern(Predicate<Type> matcher, Action<IJoinedSubclassAttributesMapper> applier)
		{
			PatternsAppliers.JoinedSubclass.Add(new DelegatedApplier<Type, IJoinedSubclassAttributesMapper>(matcher, applier));
		}

		public void AddUnionSubclassPattern(Predicate<Type> matcher, Action<IUnionSubclassAttributesMapper> applier)
		{
			PatternsAppliers.UnionSubclass.Add(new DelegatedApplier<Type, IUnionSubclassAttributesMapper>(matcher, applier));
		}

		public HbmMapping CompileMappingFor(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<Type>(types);
			DomainInspector.AddToDomain(typeToMap);

			string defaultAssemblyName = null;
			string defaultNamespace = null;
			var firstType = typeToMap.FirstOrDefault();
			if (firstType != null && typeToMap.All(t => t.Assembly.Equals(firstType.Assembly)))
			{
				defaultAssemblyName = firstType.Assembly.GetName().Name;
			}
			if (firstType != null && typeToMap.All(t => t.Namespace.Equals(firstType.Namespace)))
			{
				defaultNamespace = firstType.Namespace;
			}
			var mapping = new HbmMapping {assembly = defaultAssemblyName, @namespace = defaultNamespace};
			foreach (var type in RootClasses(typeToMap))
			{
				MapRootClass(type, mapping);
			}
			foreach (var type in Subclasses(typeToMap))
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
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				MapSubclass(type, mapping);
			}
			else if (domainInspector.IsTablePerClass(type))
			{
				MapJoinedSubclass(type, mapping);
			}
			else if (domainInspector.IsTablePerConcreteClass(type))
			{
				MapUnionSubclass(type, mapping);
			}
		}

		private void MapUnionSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new UnionSubclassMapper(type, mapping);

			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			InvokeBeforeMapUnionSubclass(type, classMapper);
			PatternsAppliers.UnionSubclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			InvokeAfterMapUnionSubclass(type, classMapper);

			MapProperties(type, propertiesToMap, classMapper);
		}

		private void MapSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new SubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			InvokeBeforeMapSubclass(type, classMapper);
			PatternsAppliers.Subclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			InvokeAfterMapSubclass(type, classMapper);

			MapProperties(type, propertiesToMap, classMapper);
		}

		private void MapJoinedSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new JoinedSubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if(baseType != null)
				{
					classMapper.Extends(baseType);
					classMapper.Key(km => km.Column(baseType.Name.ToLowerInvariant() + "_key"));
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			InvokeBeforeMapJoinedSubclass(type, classMapper);
			PatternsAppliers.JoinedSubclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			InvokeAfterMapJoinedSubclass(type, classMapper);

			MapProperties(type, propertiesToMap, classMapper);
		}

		private Type GetEntityBaseType(Type type)
		{
			Type analizingType = type;
			while (analizingType != null && analizingType != typeof (object))
			{
				analizingType = analizingType.BaseType;
				if (domainInspector.IsEntity(analizingType))
				{
					return analizingType;
				}
			}
			return type.GetInterfaces().FirstOrDefault(i => domainInspector.IsEntity(i));
		}

		private void MapRootClass(Type type, HbmMapping mapping)
		{
			var poidPropertyOrField = membersProvider.GetEntityMembersForPoid(type).FirstOrDefault(mi => domainInspector.IsPersistentId(mi));
			var classMapper = new ClassMapper(type, mapping, poidPropertyOrField);
			MapId(classMapper, poidPropertyOrField);
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator(x => { });
			}
			var persistentProperties =
				membersProvider.GetRootEntityMembers(type).Where(
					p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)).ToArray();
			var versionMember = persistentProperties.SingleOrDefault(mi => domainInspector.IsVersion(mi));
			MapVersion(classMapper, versionMember);

			InvokeBeforeMapClass(type, classMapper);
			PatternsAppliers.RootClass.ApplyAllMatchs(type, classMapper);
			InvokeClassCustomizers(type, classMapper);
			InvokeAfterMapClass(type, classMapper);

			var naturalIdPropeties = persistentProperties.Where(mi => domainInspector.IsMemberOfNaturalId(mi)).ToArray();
			if (naturalIdPropeties.Length > 0)
			{
				classMapper.NaturalId(naturalIdMapper =>
					{
						foreach (PropertyInfo property in naturalIdPropeties)
						{
							MapNaturalIdProperties(type, naturalIdMapper, property);
						}
					});
			}

			MapProperties(type, persistentProperties.Where(mi => !domainInspector.IsVersion(mi)).Except(naturalIdPropeties), classMapper);
		}

		private void MapVersion(ClassMapper classMapper, MemberInfo versionMember)
		{
			if (versionMember!= null)
			{
				classMapper.Version(versionMember, versionMapper =>
				                                   {
				                                   	PatternsAppliers.Version.ApplyAllMatchs(versionMember, versionMapper);
				                                   });
			}
		}

		private void MapId(ClassMapper classMapper, MemberInfo poidPropertyOrField)
		{
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
		}

		private void InvokeClassCustomizers(Type type, IClassMapper classMapper)
		{
			InvokeAncestorsCustomizers(type.GetInterfaces(), classMapper);
			InvokeAncestorsCustomizers(type.GetHierarchyFromBase(), classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
		}

		private void InvokeAncestorsCustomizers(IEnumerable<Type> typeAncestors, IClassMapper classMapper)
		{
			// only apply the polymorphic mapping for no entities:
			// this is to avoid a possible caos in entity-subclassing:
			// an example of caos is when a base class has a specific TableName and a subclass does not have a specific name (use the default class name).
			// I can remove this "limitation", where required, and delegate to the user the responsibility of his caos.
			foreach (var entityType in typeAncestors.Where(t => !domainInspector.IsEntity(t)))
			{
				customizerHolder.InvokeCustomizers(entityType, classMapper);
			}
		}

		private void MapNaturalIdProperties(Type rootEntityType, INaturalIdMapper naturalIdMapper, PropertyInfo property)
		{
			MemberInfo member = property;
			Type propertyType = property.GetPropertyOrFieldType();
			var memberPath = new PropertyPath(null, member);
			if (domainInspector.IsComplex(member))
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
			else if (domainInspector.IsHeterogeneousAssociation(member))
			{
				MapAny(member, memberPath, naturalIdMapper);
			}
			else if (domainInspector.IsManyToOne(rootEntityType, propertyType))
			{
				MapManyToOne(member, memberPath, propertyType, naturalIdMapper, rootEntityType);
			}
			else if (domainInspector.IsComponent(propertyType))
			{
				MapComponent(member, memberPath, propertyType, naturalIdMapper, rootEntityType);
			}
			else if (domainInspector.IsOneToOne(rootEntityType, propertyType) || domainInspector.IsSet(property)
			         || domainInspector.IsDictionary(property) || domainInspector.IsArray(property)
			         || domainInspector.IsList(property) || domainInspector.IsBag(property))
			{
				throw new ArgumentOutOfRangeException("property",
				                                      string.Format("The property {0} of {1} can't be part of natural-id.",
				                                                    property.Name, property.DeclaringType));
			}
			else
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
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

		private void MapProperties(Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
															 IPropertyContainerMapper propertiesContainer)
		{
			MapProperties(propertiesContainerType, propertiesToMap, propertiesContainer, null);			
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
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
				else if (domainInspector.IsHeterogeneousAssociation(member))
				{
					MapAny(member, memberPath, propertiesContainer);
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
				else if (domainInspector.IsArray(property))
				{
					throw new NotSupportedException();
				}
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

		private void MapAny(MemberInfo member, PropertyPath memberPath, IBasePlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Any(member, typeof(int), anyMapper =>
			{
				InvokeBeforeMapAny(memberPath, anyMapper);
				var poidPropertyOrField =
					membersProvider.GetEntityMembersForPoid(memberPath.GetRootMember().DeclaringType).FirstOrDefault(
						mi => domainInspector.IsPersistentId(mi));

				if (poidPropertyOrField != null)
				{
					anyMapper.IdType(poidPropertyOrField.GetPropertyOrFieldType());
				}
				PatternsAppliers.Any.ApplyAllMatchs(member, anyMapper);
				PatternsAppliers.AnyPath.ApplyAllMatchs(memberPath, anyMapper);
				ForEachMemberPath(member, memberPath, pp => customizerHolder.InvokeCustomizers(pp, anyMapper));
				InvokeAfterMapAny(memberPath, anyMapper);
			});
		}

		private void MapProperty(MemberInfo member, PropertyPath propertyPath, IBasePlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Property(member, propertyMapper =>
				{
					InvokeBeforeMapProperty(propertyPath, propertyMapper);
					PatternsAppliers.Property.ApplyAllMatchs(member, propertyMapper);
					PatternsAppliers.PropertyPath.ApplyAllMatchs(propertyPath, propertyMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, propertyMapper));
					InvokeAfterMapProperty(propertyPath, propertyMapper);
				});
		}

		protected void ForEachMemberPath(MemberInfo member, PropertyPath progressivePath, Action<PropertyPath> invoke)
		{
			// To ensure that a customizer is called just once I can't use a set because all customizers
			// needs to be called in a certain sequence starting from the most general (interfaces) to the
			// most specific (on progressivePath).

			// There are three levels of direct-customization of a property of the class under-mapping:
			// 1) at interface level
			// 2) at class implementation level
			// 3) at inherited class implementation level
			// After these three levels we have a special behavior for componets mapping.
			// As example :
			// We three components classes named C1, C2, C3. The C1 has a property of C2 and C2 has a property of C3.
			// Given a class X with a collection or a property of type C1 we can customize C3 starting from:
			// a) C3 itself (the cases 1-2-3 above)
			// b) from C2
			// c) from C1 then from C2
			// d) from the collection in X then C1 then from C2
			// The full-progressive-path of the property is : X.Collection->C1.C2.C3.MyProp
			// I have to execute the customization at each possible level (a,b,c,d).

			var invokedPaths = new HashSet<PropertyPath>();

			// paths on interfaces (note: when a property is the implementation of more then one interface a specific order can't be applied...AFAIK)
			var propertiesOnInterfaces = member.GetPropertyFromInterfaces();
			foreach (var propertyOnInterface in propertiesOnInterfaces)
			{
				var propertyPathInterfaceLevel = new PropertyPath(null, propertyOnInterface);
				invoke(propertyPathInterfaceLevel);
			}

			// path on declaring type
			var propertyPathLevel0 = new PropertyPath(null, member.GetMemberFromDeclaringType());
			// path on reflected type
			var propertyPathLevel1 = new PropertyPath(null, member);

			invoke(propertyPathLevel0); invokedPaths.Add(propertyPathLevel0);

			if(!propertyPathLevel0.Equals(propertyPathLevel1))
			{
				invoke(propertyPathLevel1); invokedPaths.Add(propertyPathLevel1);
			}

			foreach (var propertyPath in progressivePath.InverseProgressivePath())
			{
				if (!invokedPaths.Contains(propertyPath))
				{
					invoke(propertyPath);
					invokedPaths.Add(propertyPath);
				}
			}
		}

		private void MapComponent(MemberInfo member, PropertyPath memberPath, Type propertyType, IBasePlainPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.Component(member, componentMapper =>
				{
					InvokeBeforeMapComponent(memberPath, componentMapper);
					Type componentType = propertyType;
					IEnumerable<MemberInfo> persistentProperties =
						membersProvider.GetComponentMembers(componentType).Where(p => domainInspector.IsPersistentProperty(p));

					MemberInfo parentReferenceProperty = domainInspector.GetBidirectionalMember(propertiesContainerType, member, componentType) ??
					                                     persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);
					if (parentReferenceProperty != null)
					{
						componentMapper.Parent(parentReferenceProperty,
						                       componentParentMapper =>
						                       patternsAppliers.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
						                                                                       componentParentMapper));
					}
					PatternsAppliers.Component.ApplyAllMatchs(componentType, componentMapper);
					PatternsAppliers.ComponentProperty.ApplyAllMatchs(member, componentMapper);
					PatternsAppliers.ComponentPropertyPath.ApplyAllMatchs(memberPath, componentMapper);
					customizerHolder.InvokeCustomizers(componentType, componentMapper);
					ForEachMemberPath(member, memberPath, pp=> customizerHolder.InvokeCustomizers(pp, componentMapper));
					InvokeAfterMapComponent(memberPath, componentMapper);

					MapProperties(propertyType, persistentProperties.Where(pi => pi != parentReferenceProperty), componentMapper, memberPath);
				});
		}

		private void MapBag(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Bag(member, collectionPropertiesMapper =>
				{
					InvokeBeforeMapBag(propertyPath, collectionPropertiesMapper);
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Bag.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.BagPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
					InvokeAfterMapBag(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapList(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                     Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.List(member, collectionPropertiesMapper =>
				{
					InvokeBeforeMapList(propertyPath, collectionPropertiesMapper);
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.List.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.ListPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
					InvokeAfterMapList(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapDictionary(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                           Type propertiesContainerType)
		{
			Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			if (dictionaryKeyType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              member.Name, propertiesContainerType));
			}
			IMapKeyRelationMapper mkrm = DetermineMapKeyRelationType(member, propertyPath, dictionaryKeyType);

			Type dictionaryValueType = propertyType.DetermineDictionaryValueType();
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, dictionaryValueType);
			
			propertiesContainer.Map(member, collectionPropertiesMapper =>
				{
					InvokeBeforeMapMap(propertyPath, collectionPropertiesMapper);
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Map.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.MapPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
					InvokeAfterMapMap(propertyPath, collectionPropertiesMapper);
				}, mkrm.Map, cert.Map);
		}

		private void MapSet(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Set(member, collectionPropertiesMapper =>
				{
					InvokeBeforeMapSet(propertyPath, collectionPropertiesMapper);
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Set.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.SetPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
					InvokeAfterMapSet(propertyPath, collectionPropertiesMapper);
				}, cert.Map);
		}

		private void MapOneToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                         Type propertiesContainerType)
		{
			propertiesContainer.OneToOne(member, oneToOneMapper =>
				{
					InvokeBeforeMapOneToOne(propertyPath, oneToOneMapper);
					CascadeOn? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					oneToOneMapper.Cascade(cascade.GetValueOrDefault(CascadeOn.None).ToCascade());
					PatternsAppliers.OneToOne.ApplyAllMatchs(member, oneToOneMapper);
					PatternsAppliers.OneToOnePath.ApplyAllMatchs(propertyPath, oneToOneMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, oneToOneMapper));
					InvokeAfterMapOneToOne(propertyPath, oneToOneMapper);
				});
		}

		private void MapManyToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IBasePlainPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.ManyToOne(member, manyToOneMapper =>
				{
					InvokeBeforeMapManyToOne(propertyPath, manyToOneMapper);
					CascadeOn? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					manyToOneMapper.Cascade(cascade.GetValueOrDefault(CascadeOn.None).ToCascade());
					PatternsAppliers.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
					PatternsAppliers.ManyToOnePath.ApplyAllMatchs(propertyPath, manyToOneMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, manyToOneMapper));
					InvokeAfterMapManyToOne(propertyPath, manyToOneMapper);
				});
		}

		private Type GetCollectionElementTypeOrThrow(Type type, MemberInfo property, Type propertyType)
		{
			Type collectionElementType = propertyType.DetermineCollectionElementType();
			if (collectionElementType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              property.Name, type));
			}
			return collectionElementType;
		}

		protected interface IMapKeyRelationMapper
		{
			void Map(IMapKeyRelation relation);
		}

		protected interface ICollectionElementRelationMapper
		{
			void Map(ICollectionElementRelation relation);
			void MapCollectionProperties(ICollectionPropertiesMapper mapped);
		}

		private class ElementRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public ElementRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Element(x =>
					{
						mapper.InvokeBeforeMapElement(propertyPath, x);
						appliers.Element.ApplyAllMatchs(member, x);
						appliers.ElementPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
						mapper.InvokeAfterMapElement(propertyPath, x);
					});
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
			private readonly PropertyPath propertyPath;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public OneToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, Type ownerType, Type collectionElementType, IDomainInspector domainInspector, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.OneToMany(x =>
					{
						mapper.InvokeBeforeMapOneToMany(propertyPath, x);
						appliers.OneToMany.ApplyAllMatchs(member, x);
						appliers.OneToManyPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
						mapper.InvokeAfterMapOneToMany(propertyPath, x);
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var parentColumnNameInChild = GetParentColumnNameInChild();
				if (parentColumnNameInChild != null)
				{
					mapped.Key(k => k.Column(parentColumnNameInChild));
				}
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				mapped.Cascade(cascadeToApply.GetValueOrDefault(CascadeOn.None).ToCascade());
			}

			private string GetParentColumnNameInChild()
			{
				MemberInfo propertyInfo = domainInspector.GetBidirectionalMember(ownerType, member, collectionElementType) ??
				                          collectionElementType.GetProperties(FlattenHierarchyBindingFlags).FirstOrDefault(p => p.PropertyType.IsAssignableFrom(ownerType) && domainInspector.IsPersistentProperty(p));
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
			private readonly PropertyPath propertyPath;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public ManyToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, Type ownerType, Type collectionElementType, IDomainInspector domainInspector, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToMany(x =>
					{
						mapper.InvokeBeforeMapManyToMany(propertyPath, x);
						appliers.ManyToMany.ApplyAllMatchs(member, x);
						appliers.ManyToManyPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
						mapper.InvokeAfterMapManyToMany(propertyPath, x);
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				mapped.Cascade(cascadeToApply.GetValueOrDefault(CascadeOn.None).ToCascade());
			}

			#endregion
		}

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo collectionMember;
			private readonly Type ownerType;
			private readonly Type componentType;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public ComponentRelationMapper(MemberInfo collectionMember, Type ownerType, Type componentType, ICandidatePersistentMembersProvider membersProvider, IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.collectionMember = collectionMember;
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Component(x =>
					{
						var persistentProperties = GetPersistentProperties(componentType);
						MemberInfo parentReferenceProperty = domainInspector.GetBidirectionalMember(ownerType, collectionMember, componentType) ??
						                                     persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == ownerType);
						if (parentReferenceProperty != null)
						{
							x.Parent(parentReferenceProperty,
							         componentParentMapper =>
							         patternsAppliersHolder.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
							                                                               componentParentMapper));
						}
						patternsAppliersHolder.Component.ApplyAllMatchs(componentType, x);
						customizersHolder.InvokeCustomizers(componentType, x);

						PropertyPath propertyPath = new PropertyPath(null, collectionMember);
						MapProperties(componentType, propertyPath, x, persistentProperties.Where(pi => pi != parentReferenceProperty));
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion

			private IEnumerable<MemberInfo> GetPersistentProperties(Type type)
			{
				var properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(Type type, PropertyPath memberPath, IComponentElementMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				// TODO check PropertyPath behaviour when the component is in a collection
				foreach (var property in persistentProperties)
				{
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					var propertyPath = new PropertyPath(memberPath, member);

					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
							{
								mapper.InvokeBeforeMapManyToOne(propertyPath, manyToOneMapper);
								patternsAppliersHolder.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
								patternsAppliersHolder.ManyToOnePath.ApplyAllMatchs(propertyPath, manyToOneMapper);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
								mapper.InvokeAfterMapManyToOne(propertyPath, manyToOneMapper);
							});
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(member, x =>
							{
								mapper.InvokeBeforeMapComponent(propertyPath, x);
								// Note: for nested-components the Parent discovering is mandatory (recursive nested-component); 
								// for the same reason you can't have more than one property of the type of the Parent component
								var componentOwnerType = type;
								var componentPropertyType = propertyType;

								var componentProperties = GetPersistentProperties(componentPropertyType);
								var parentReferenceProperty = componentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == componentOwnerType);
								if (parentReferenceProperty != null)
								{
									x.Parent(parentReferenceProperty,
									         componentParentMapper =>
									         patternsAppliersHolder.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
									                                                               componentParentMapper));
								}
								patternsAppliersHolder.Component.ApplyAllMatchs(componentPropertyType, x);
								patternsAppliersHolder.ComponentProperty.ApplyAllMatchs(member, x);
								patternsAppliersHolder.ComponentPropertyPath.ApplyAllMatchs(propertyPath, x);
								customizersHolder.InvokeCustomizers(componentPropertyType, x);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, x));
								mapper.InvokeAfterMapComponent(propertyPath, x);

								MapProperties(componentPropertyType, propertyPath, x, componentProperties.Where(pi => pi != parentReferenceProperty));
							});
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
							{
								mapper.InvokeBeforeMapProperty(propertyPath, propertyMapper);
								patternsAppliersHolder.Property.ApplyAllMatchs(member, propertyMapper);
								patternsAppliersHolder.PropertyPath.ApplyAllMatchs(propertyPath, propertyMapper);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
								mapper.InvokeAfterMapProperty(propertyPath, propertyMapper);
							});
					}
				}
			}
		}

		protected virtual ICollectionElementRelationMapper DetermineCollectionElementRelationType(MemberInfo property, PropertyPath propertyPath, Type collectionElementType)
		{
			var ownerType = property.ReflectedType;
			if (domainInspector.IsOneToMany(ownerType, collectionElementType))
			{
				return new OneToManyRelationMapper(property, propertyPath, ownerType, collectionElementType, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			if (domainInspector.IsManyToMany(ownerType, collectionElementType))
			{
				return new ManyToManyRelationMapper(property, propertyPath, ownerType, collectionElementType, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			if (domainInspector.IsComponent(collectionElementType))
			{
				return new ComponentRelationMapper(property, ownerType, collectionElementType, membersProvider, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			return new ElementRelationMapper(property, propertyPath, PatternsAppliers, customizerHolder, this);
		}

		private IMapKeyRelationMapper DetermineMapKeyRelationType(MemberInfo member, PropertyPath propertyPath, Type dictionaryKeyType)
		{
			var ownerType = member.ReflectedType;
			if (domainInspector.IsManyToMany(ownerType, dictionaryKeyType) || domainInspector.IsOneToMany(ownerType, dictionaryKeyType))
			{
				// OneToMany is not possible as map-key so we map it as many-to-many instead ignore the case
				return new KeyManyToManyRelationMapper(member, propertyPath, PatternsAppliers, customizerHolder, this);
			}
			if (domainInspector.IsComponent(dictionaryKeyType))
			{
				return new KeyComponentRelationMapper(dictionaryKeyType, propertyPath, membersProvider, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			return new KeyElementRelationMapper(member, propertyPath, PatternsAppliers, customizerHolder, this);
		}

		private class KeyElementRelationMapper : IMapKeyRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public KeyElementRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.Element(x=>
				                 	{
														mapper.InvokeBeforeMapMapKey(propertyPath, x);
														patternsAppliersHolder.MapKey.ApplyAllMatchs(member, x);
														patternsAppliersHolder.MapKeyPath.ApplyAllMatchs(propertyPath, x);
														customizersHolder.InvokeCustomizers(propertyPath, x);
														mapper.InvokeAfterMapMapKey(propertyPath, x);
													});
			}
		}

		private class KeyComponentRelationMapper : IMapKeyRelationMapper
		{
			private readonly Type dictionaryKeyType;
			private readonly PropertyPath propertyPath;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public KeyComponentRelationMapper(Type dictionaryKeyType, PropertyPath propertyPath, ICandidatePersistentMembersProvider membersProvider, IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.dictionaryKeyType = dictionaryKeyType;
				this.propertyPath = propertyPath;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.Component(x =>
				{
					var persistentProperties = GetPersistentProperties(dictionaryKeyType);

					MapProperties(dictionaryKeyType, x, persistentProperties);
				});
			}

			private IEnumerable<MemberInfo> GetPersistentProperties(Type type)
			{
				var properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(Type type, IComponentMapKeyMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				foreach (var property in persistentProperties)
				{
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
						                                      	{
																											var progressivePath = new PropertyPath(propertyPath, member);
																											mapper.InvokeBeforeMapManyToOne(progressivePath, manyToOneMapper);
																											patternsAppliersHolder.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
																											mapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
																											mapper.InvokeAfterMapManyToOne(progressivePath, manyToOneMapper);
																										});
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
						                                     	{
																										var progressivePath = new PropertyPath(propertyPath, member);
																										mapper.InvokeBeforeMapProperty(progressivePath, propertyMapper);
																										patternsAppliersHolder.Property.ApplyAllMatchs(member, propertyMapper);
																										mapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
																										mapper.InvokeAfterMapProperty(progressivePath, propertyMapper);
																									});
					}
				}
			}
		}

		private class KeyManyToManyRelationMapper : IMapKeyRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder patternsAppliers;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public KeyManyToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder patternsAppliers, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.patternsAppliers = patternsAppliers;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.ManyToMany(x=>
				                    	{
																mapper.InvokeBeforeMapMapKeyManyToMany(propertyPath, x);
																patternsAppliers.MapKeyManyToMany.ApplyAllMatchs(member, x);
																patternsAppliers.MapKeyManyToManyPath.ApplyAllMatchs(propertyPath, x);
																customizersHolder.InvokeCustomizers(propertyPath, x);
																mapper.InvokeAfterMapMapKeyManyToMany(propertyPath, x);
															});
			}
		}

		public IEnumerable<HbmMapping> CompileMappingForEach(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<Type>(types);
			DomainInspector.AddToDomain(typeToMap);

			foreach (var type in RootClasses(typeToMap))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				MapRootClass(type, mapping);
				yield return mapping;
			}
			foreach (var type in Subclasses(typeToMap))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				AddSubclassMapping(mapping, type);
				yield return mapping;
			}
		}

		public void TypeDef<TComplex, TUserType>() where TUserType: IUserType
		{
			TypeDef(typeof(TComplex), typeof(TUserType));
		}

		public void TypeDef(Type typeOfComplex, Type typeOfCustomPersistentType)
		{
			AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeOfComplex, pm => pm.Type(typeOfCustomPersistentType, null));
			PatternsAppliers.Element.Add(new CustomUserTypeInCollectionElementApplier(typeOfComplex, typeOfCustomPersistentType));
			PatternsAppliers.MapKey.Add(new CustomUserTypeInDictionaryKeyApplier(typeOfComplex, typeOfCustomPersistentType));
		}
	}
}