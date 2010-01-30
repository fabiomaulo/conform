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
		private readonly HashSet<Type> rootEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassHierarchyEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerConcreteClassEntities = new HashSet<Type>();
		private readonly HashSet<Relation> manyToOneRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToManyRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> oneToOneRelation = new HashSet<Relation>();
		private readonly HashSet<Relation> manyToManyRelation = new HashSet<Relation>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly List<IPattern<MemberInfo>> poidPatterns;
		private readonly List<IPattern<MemberInfo>> setPatterns;
		private readonly HashSet<Type> components = new HashSet<Type>();
		
		public ObjectRelationalMapper()
		{
			poidPatterns = new List<IPattern<MemberInfo>> {new PoIdPattern()};
			setPatterns = new List<IPattern<MemberInfo>> { new SetCollectionPattern() };
		}

		#region Implementation of IObjectRelationalMapper

		public void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			if (IsComponent(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			rootEntities.Add(type);
			tablePerClassHierarchyEntities.Add(type);
		}

		public void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			if (IsComponent(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			rootEntities.Add(type);
			tablePerClassEntities.Add(type);
		}

		public void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			if (IsComponent(type))
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

		public void AsSet<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			sets.Add(member);
		}

		#endregion

		#region Implementation of IDomainInspector

		public bool IsRootEntity(Type type)
		{
			return rootEntities.Contains(type);
		}

		public bool IsComponent(Type type)
		{
			return components.Contains(type);
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
			return (areEntities && manyToOneRelation.Contains(new Relation(from, to))) || (areEntities && !IsOneToOne(from, to));
		}

		public bool IsManyToMany(Type role1, Type role2)
		{
			return IsEntity(role1) && IsEntity(role2) && manyToManyRelation.Contains(new Relation(role1, role2));
		}

		public bool IsOneToMany(Type from, Type to)
		{
			var areEntities = IsEntity(from) && IsEntity(to);
			return (areEntities && oneToManyRelation.Contains(new Relation(from, to))) || (areEntities && !IsOneToOne(from, to));
		}

		public bool IsHeterogeneousAssociations(MemberInfo member)
		{
			throw new NotImplementedException();
		}

		public bool IsPersistentId(MemberInfo member)
		{
			return poidPatterns.Any(pattern => pattern.Match(member));
		}

		public bool IsPersistentProperty(MemberInfo role)
		{
			return true;
		}

		public IDbColumnSpecification[] GetPersistentSpecification(MemberInfo role)
		{
			throw new NotImplementedException();
		}

		public bool IsSet(MemberInfo role)
		{
			return sets.Contains(role) || setPatterns.Any(sp => sp.Match(role)); 
		}

		public bool IsBag(MemberInfo role)
		{
			throw new NotImplementedException();
		}

		public bool IsList(MemberInfo role)
		{
			throw new NotImplementedException();
		}

		public bool IsArray(MemberInfo role)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}