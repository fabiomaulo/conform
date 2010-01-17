using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm
{
	public class ObjectRelationalMapper : IObjectRelationalMapper, IDomainInspector
	{
		private readonly HashSet<Type> rootEntities = new HashSet<Type>();
		private readonly HashSet<Type> tablePerClassEntities = new HashSet<Type>();
		private HashSet<Type> tablePerClassHierarchyEntities = new HashSet<Type>();
		private HashSet<Type> tablePerConcreteClassEntities = new HashSet<Type>();
		#region Implementation of IObjectRelationalMapper

		public void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			throw new NotImplementedException();
		}

		public void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			rootEntities.Add(type);
			tablePerClassEntities.Add(type);
		}

		public void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			throw new NotImplementedException();
		}

		public void ValueObject<TComponent>()
		{
			throw new NotImplementedException();
		}

		public void ManyToMany<TLeftEntity, TRigthEntity>()
		{
			throw new NotImplementedException();
		}

		public void ManyToOne<TLeftEntity, TRigthEntity>()
		{
			throw new NotImplementedException();
		}

		public void OneToMany<TLeftEntity, TRigthEntity>()
		{
			throw new NotImplementedException();
		}

		public void OneToOne<TLeftEntity, TRigthEntity>()
		{
			throw new NotImplementedException();
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
			if (!isExplicitTablePerClass)
			{
				var derived = type.GetBaseTypes().Any(t => tablePerClassEntities.Contains(t));
				if(derived)
				{
					tablePerClassEntities.Add(type);
				}
			}
			return isExplicitTablePerClass;
		}

		public bool IsTablePerHierarchy(Type type)
		{
			return tablePerClassHierarchyEntities.Contains(type);
		}

		public bool IsTablePerConcreteClass(Type type)
		{
			return tablePerConcreteClassEntities.Contains(type);
		}

		public bool IsOneToOne(Type from, Type to)
		{
			throw new NotImplementedException();
		}

		public bool IsManyToOne(Type from, Type to)
		{
			throw new NotImplementedException();
		}

		public bool IsManyToMany(Type role1, Type role2)
		{
			throw new NotImplementedException();
		}

		public bool IsManyToMany(Type role1, Type role2, MemberInfo relationOwnerRole)
		{
			throw new NotImplementedException();
		}

		public bool IsOneToMany(Type from, Type to)
		{
			throw new NotImplementedException();
		}

		public bool IsOneToMany(Type from, Type to, MemberInfo toRole)
		{
			throw new NotImplementedException();
		}

		public bool IsHeterogeneousAssociations(MemberInfo member)
		{
			throw new NotImplementedException();
		}

		public bool IsPersistentId(MemberInfo member)
		{
			throw new NotImplementedException();
		}

		public bool IsPersistentProperty(MemberInfo role)
		{
			throw new NotImplementedException();
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