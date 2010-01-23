using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly List<IPattern<MemberInfo>> poidPatterns;
		
		public ObjectRelationalMapper()
		{
			poidPatterns = new List<IPattern<MemberInfo>> {new PoIdPattern()};
		}

		#region Implementation of IObjectRelationalMapper

		public void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			rootEntities.Add(type);
			tablePerClassHierarchyEntities.Add(type);
		}

		public void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			rootEntities.Add(type);
			tablePerClassEntities.Add(type);
		}

		public void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			rootEntities.Add(type);
			tablePerConcreteClassEntities.Add(type);
		}

		public void Component<TComponent>()
		{
			throw new NotImplementedException();
		}

		public void ManyToMany<TLeftEntity, TRigthEntity>()
		{
			throw new NotImplementedException();
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

		#endregion

		#region Implementation of IDomainInspector

		public bool IsRootEntity(Type type)
		{
			return rootEntities.Contains(type);
		}

		public bool IsComponent(Type type)
		{
			return false;
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
			var isExplicitTablePerClass = tablePerClassEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitTablePerClass)
			{
				isDerived = type.GetBaseTypes().Any(t => tablePerClassEntities.Contains(t));
				if(isDerived)
				{
					tablePerClassEntities.Add(type);
				}
			}
			return isExplicitTablePerClass || isDerived;
		}

		public bool IsTablePerClassHierarchy(Type type)
		{
			var isExplicitTablePerClassHierarchy = tablePerClassHierarchyEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitTablePerClassHierarchy)
			{
				isDerived = type.GetBaseTypes().Any(t => tablePerClassHierarchyEntities.Contains(t));
				if (isDerived)
				{
					tablePerClassHierarchyEntities.Add(type);
				}
			}
			return isExplicitTablePerClassHierarchy || isDerived;
		}

		public bool IsTablePerConcreteClass(Type type)
		{
			var isExplicitTablePerConcreteClass = tablePerConcreteClassEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitTablePerConcreteClass)
			{
				isDerived = type.GetBaseTypes().Any(t => tablePerConcreteClassEntities.Contains(t));
				if (isDerived)
				{
					tablePerConcreteClassEntities.Add(type);
				}
			}
			return isExplicitTablePerConcreteClass || isDerived;
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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