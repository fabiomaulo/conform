using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Patterns;

namespace ConfOrm
{
	public class ObjectRelationalMapper : IObjectRelationalMapper, IDomainInspector
	{
		#region Explicit mappings

		private readonly HashSet<Type> rootEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassHierarchyEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerConcreteClassEntities = new HashSet<Type>();
		private readonly HashSet<Relation> manyToOneRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToManyRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToOneRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> manyToManyRelation = new HashSet<Relation>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
		private readonly HashSet<Type> components = new HashSet<Type>();
		private readonly Dictionary<Relation, Cascade> cascade = new Dictionary<Relation, Cascade>();
		private readonly HashSet<MemberInfo> persistentProperty = new HashSet<MemberInfo>();
		private readonly HashSet<Type> complex = new HashSet<Type>();
		private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();

		#endregion

		private readonly IPatternsHolder patterns;

		public ObjectRelationalMapper() : this(new PatternsHolder()) {}

		public ObjectRelationalMapper(IPatternsHolder patterns)
		{
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns");
			}
			this.patterns = patterns;
			patterns.Poids.Add(new PoIdPattern());
			patterns.Sets.Add(new SetCollectionPattern());
			patterns.Bags.Add(new BagCollectionPattern());
			patterns.Lists.Add(new ListCollectionPattern(this));
			patterns.Arrays.Add(new ArrayCollectionPattern());
			patterns.Componets.Add(new ComponentPattern());
			patterns.Dictionaries.Add(new DictionaryCollectionPattern());
			patterns.Cascades.Add(new BidirectionalOneToManyCascadePattern(this));

			patterns.PoidStrategies.Add(new HighLowPoidPattern());
			patterns.PoidStrategies.Add(new GuidOptimizedPoidPattern());

			patterns.PersistentPropertiesExclusions.Add(new ReadOnlyPropertyPattern());
		}

		public IPatternsHolder Patterns
		{
			get { return patterns; }
		}

		#region Implementation of IObjectRelationalMapper

		public void TablePerClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClass(baseEntity);
			}
		}

		public void TablePerClassHierarchy(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClassHierarchy(baseEntity);
			}
		}

		public void TablePerConcreteClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerConcreteClass(baseEntity);
			}
		}

		public void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerClassHierarchy(type);
		}

		private void RegisterTablePerClassHierarchy(Type type)
		{
			PreventComponentAsEntity(type);
			rootEntities.Add(type);
			tablePerClassHierarchyEntities.Add(type);
		}

		private void PreventComponentAsEntity(Type type)
		{
			if (components.Contains(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
		}

		public void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			RegisterTablePerClass(type);
		}

		private void RegisterTablePerClass(Type type)
		{
			PreventComponentAsEntity(type);
			rootEntities.Add(type);
			tablePerClassEntities.Add(type);
		}

		public void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerConcreteClass(type);
		}

		private void RegisterTablePerConcreteClass(Type type)
		{
			PreventComponentAsEntity(type);
			rootEntities.Add(type);
			tablePerConcreteClassEntities.Add(type);
		}

		public void Component<TComponent>()
		{
			var type = typeof(TComponent);
			if(IsEntity(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			components.Add(type);
		}

		public void Complex<TComplex>()
		{
			var type = typeof(TComplex);
			complex.Add(type);
		}

		public void Poid<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			poids.Add(member);
		}

		public void ManyToMany<TLeftEntity, TRigthEntity>()
		{
			manyToManyRelation.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			manyToManyRelation.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity)));
		}

		public void ManyToOne<TLeftEntity, TRigthEntity>()
		{
			manyToOneRelation.Add(new Relation(typeof (TLeftEntity), typeof (TRigthEntity)));
			oneToManyRelation.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity)));
		}

		public void OneToOne<TLeftEntity, TRigthEntity>()
		{
			oneToOneRelation.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			oneToOneRelation.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity)));
		}

		public void Set<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			sets.Add(member);
		}

		public void Bag<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			bags.Add(member);
		}

		public void List<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			lists.Add(member);
		}

		public void Array<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			arrays.Add(member);
		}

		public void Dictionary<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			dictionaries.Add(member);
		}

		public void Cascade<TFromEntity, TToEntity>(Cascade cascadeOptions)
		{
			cascade.Add(new Relation(typeof(TFromEntity), typeof(TToEntity)), cascadeOptions);
		}

		public void PersistentProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			persistentProperty.Add(member);
		}

		#endregion

		#region Implementation of IDomainInspector

		public bool IsRootEntity(Type type)
		{
			return rootEntities.Contains(type);
		}

		public bool IsComponent(Type type)
		{
			return (components.Contains(type) || (Patterns.Componets.Match(type) && !IsEntity(type))) && !complex.Contains(type);
		}

		public bool IsComplex(Type type)
		{
			return complex.Contains(type);
		}

		public bool IsEntity(Type type)
		{
			return rootEntities.Contains(type) || type.GetBaseTypes().Any(t => rootEntities.Contains(t));
		}

		public bool IsTablePerClass(Type type)
		{
			return IsMappedFor(tablePerClassEntities, type);
		}

		public bool IsTablePerClassHierarchy(Type type)
		{
			return IsMappedFor(tablePerClassHierarchyEntities, type);
		}

		private bool IsMappedFor(ICollection<Type> explicitMappedEntities, Type type)
		{
			var isExplicitMapped = explicitMappedEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitMapped)
			{
				isDerived = type.GetBaseTypes().Any(explicitMappedEntities.Contains);
				if (isDerived)
				{
					explicitMappedEntities.Add(type);
				}
			}
			return isExplicitMapped || isDerived;
		}

		public bool IsTablePerConcreteClass(Type type)
		{
			return IsMappedFor(tablePerConcreteClassEntities, type);
		}

		public bool IsOneToOne(Type from, Type to)
		{
			return IsEntity(from) && IsEntity(to) && oneToOneRelation.Contains(new Relation(from, to));
		}

		public bool IsManyToOne(Type from, Type to)
		{
			var areEntities = IsEntity(from) && IsEntity(to);
			var isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			return (areEntities && manyToOneRelation.Contains(new Relation(from, to)))
			       || (areEntities && !IsOneToOne(from, to) && !manyToManyRelation.Contains(new Relation(from, to))) ||
						 isFromComponentToEntity;
		}

		public bool IsManyToMany(Type role1, Type role2)
		{
			return IsEntity(role1) && IsEntity(role2) && manyToManyRelation.Contains(new Relation(role1, role2));
		}

		public bool IsOneToMany(Type from, Type to)
		{
			if(IsEntity(to) && oneToManyRelation.Contains(new Relation(from, to)))
			{
				return true;
			}
			var areEntities = IsEntity(from) && IsEntity(to);
			var isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			return (areEntities || isFromComponentToEntity) && !IsOneToOne(from, to) && !manyToManyRelation.Contains(new Relation(from, to));
		}

		public bool IsHeterogeneousAssociations(MemberInfo member)
		{
			return false;
		}

		public Cascade ApplyCascade(Type from, MemberInfo on, Type to)
		{
			var relationOn = new RelationOn(from, on, to);

			Cascade resultByClasses;
			var relation = new Relation(from, to);
			if (cascade.TryGetValue(relation, out resultByClasses))
			{
				// Has Explicit Cascade By Classes
				return resultByClasses;
			}

			return Patterns.Cascades.GetValueOfFirstMatch(relationOn);
		}

		public bool IsPersistentId(MemberInfo member)
		{
			return poids.Contains(member) || Patterns.Poids.Match(member);
		}

		public IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member)
		{
			return Patterns.PoidStrategies.GetValueOfFirstMatch(member);
		}

		public bool IsPersistentProperty(MemberInfo role)
		{
			return !Patterns.PersistentPropertiesExclusions.Match(role) || persistentProperty.Contains(role);
		}

		public bool IsSet(MemberInfo role)
		{
			return sets.Contains(role) || Patterns.Sets.Match(role); 
		}

		public bool IsBag(MemberInfo role)
		{
			return bags.Contains(role) || (!sets.Contains(role) && !lists.Contains(role) && !arrays.Contains(role) && Patterns.Bags.Match(role));
		}

		public bool IsList(MemberInfo role)
		{
			return lists.Contains(role) || (!arrays.Contains(role) && !bags.Contains(role) && Patterns.Lists.Match(role));
		}

		public bool IsArray(MemberInfo role)
		{
			return arrays.Contains(role) || (!lists.Contains(role) && !bags.Contains(role) && Patterns.Arrays.Match(role));
		}

		public bool IsDictionary(MemberInfo role)
		{
			return dictionaries.Contains(role) || Patterns.Dictionaries.Match(role);
		}

		#endregion
	}
}