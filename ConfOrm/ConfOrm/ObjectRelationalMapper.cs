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

		#endregion

		#region Patterns

		private readonly List<IPattern<MemberInfo>> poidPatterns;
		private readonly List<IPattern<MemberInfo>> setPatterns;
		private readonly List<IPattern<MemberInfo>> bagPatterns;
		private readonly List<IPattern<MemberInfo>> listPatterns;
		private readonly List<IPattern<MemberInfo>> arrayPatterns;
		private readonly List<IPattern<Type>> componetPatterns;
		private readonly List<IPattern<MemberInfo>> dictionaryPatterns;
		private readonly List<IPatternApplier<Relation, Cascade>> cascadePatterns;
		private readonly List<IPatternApplier<MemberInfo, IPersistentIdStrategy>> poidStrategyPatterns;
		#endregion

		public ObjectRelationalMapper()
		{
			poidPatterns = new List<IPattern<MemberInfo>> {new PoIdPattern()};
			setPatterns = new List<IPattern<MemberInfo>> { new SetCollectionPattern() };
			bagPatterns = new List<IPattern<MemberInfo>> { new BagCollectionPattern() };
			listPatterns = new List<IPattern<MemberInfo>> { new ListCollectionPattern() };
			arrayPatterns = new List<IPattern<MemberInfo>> { new ArrayCollectionPattern() };
			componetPatterns = new List<IPattern<Type>> { new ComponentPattern() };
			dictionaryPatterns = new List<IPattern<MemberInfo>> { new DictionaryCollectionPattern() };
			cascadePatterns = new List<IPatternApplier<Relation, Cascade>> { new BidirectionalOneToManyCascadePattern() };
			poidStrategyPatterns = new List<IPatternApplier<MemberInfo, IPersistentIdStrategy>>
			                       	{new HighLowPoidPattern(), new GuidOptimizedPoidPattern()};
		}

		public ICollection<IPatternApplier<MemberInfo, IPersistentIdStrategy>> PoidStrategies
		{
			get { return poidStrategyPatterns; }
		}

		#region Implementation of IObjectRelationalMapper

		public void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			if (components.Contains(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			rootEntities.Add(type);
			tablePerClassHierarchyEntities.Add(type);
		}

		public void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			if (components.Contains(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			rootEntities.Add(type);
			tablePerClassEntities.Add(type);
		}

		public void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			if (components.Contains(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
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

		public void Cascade<TFromEntity, TToEntity>(Expression<Func<TFromEntity, object>> propertyGetter, Cascade cascadeOptions)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			cascade.Add(new RelationOn(typeof(TFromEntity), member, typeof(TToEntity)), cascadeOptions);
		}

		#endregion

		#region Implementation of IDomainInspector

		public bool IsRootEntity(Type type)
		{
			return rootEntities.Contains(type);
		}

		public bool IsComponent(Type type)
		{
			return components.Contains(type) || (componetPatterns.Match(type) && !IsEntity(type));
		}

		public bool IsComplex(Type type)
		{
			return false;
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
			var areEntities = IsEntity(from) && IsEntity(to);
			return (areEntities && oneToManyRelation.Contains(new Relation(from, to)))
			       || (areEntities && !IsOneToOne(from, to) && !manyToManyRelation.Contains(new Relation(from, to)));
		}

		public bool IsHeterogeneousAssociations(MemberInfo member)
		{
			return false;
		}

		public Cascade ApplyCascade(Type from, MemberInfo on, Type to)
		{
			Cascade resultOnProperty;
			var relationOn = new RelationOn(from, on, to);
			if(cascade.TryGetValue(relationOn, out resultOnProperty))
			{
				// Has Explicit Cascade On Property
				return resultOnProperty;
			}

			Cascade resultByClasses;
			var relation = new Relation(from, to);
			if (cascade.TryGetValue(relation, out resultByClasses))
			{
				// Has Explicit Cascade By Classes
				return resultByClasses;
			}

			return cascadePatterns.ApplyFirstMatch(relationOn);
		}

		public bool IsPersistentId(MemberInfo member)
		{
			return poidPatterns.Match(member);
		}

		public IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member)
		{
			return poidStrategyPatterns.ApplyFirstMatch(member);
		}

		public bool IsPersistentProperty(MemberInfo role)
		{
			return true;
		}

		public IDbColumnSpecification[] GetPersistentSpecification(MemberInfo role)
		{
			return new IDbColumnSpecification[0];
		}

		public bool IsSet(MemberInfo role)
		{
			return sets.Contains(role) || setPatterns.Match(role); 
		}

		public bool IsBag(MemberInfo role)
		{
			return bags.Contains(role) || (!sets.Contains(role) && !lists.Contains(role) && !arrays.Contains(role) && bagPatterns.Match(role));
		}

		public bool IsList(MemberInfo role)
		{
			return lists.Contains(role) || (!arrays.Contains(role) && !bags.Contains(role) && listPatterns.Match(role));
		}

		public bool IsArray(MemberInfo role)
		{
			return arrays.Contains(role) || (!lists.Contains(role) && !bags.Contains(role) && arrayPatterns.Match(role));
		}

		public bool IsDictionary(MemberInfo role)
		{
			return dictionaries.Contains(role) || dictionaryPatterns.Match(role);
		}

		#endregion
	}
}