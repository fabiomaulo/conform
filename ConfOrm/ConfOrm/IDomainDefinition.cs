using System;
using System.Data;
using System.Reflection;

namespace ConfOrm
{
	public interface IDomainInspector
	{
		bool IsRootEntity(Type type);
		bool IsComponent(Type type);
		bool IsComplex(Type type);
		bool IsEntity(Type type);

		bool ApplyTablePerClass(Type type);
		bool ApplyTablePerHierarchy(Type type);
		bool ApplyTablePerConcreteClass(Type type);

		bool IsOneToOne(Type from, Type to);
		bool IsManyToOne(Type from, Type to);
		bool IsManyToMany(Type role1, Type role2);
		bool IsManyToMany(Type role1, Type role2, MemberInfo relationOwnerRole);
		bool IsOneToMany(Type from, Type to);
		bool IsOneToMany(Type from, Type to, MemberInfo toRole);
		bool IsHeterogeneousAssociations(MemberInfo member);

		bool IsPersistentId(MemberInfo member);

		bool IsPersistentProperty(MemberInfo role);
		DbType GetPersistentType(MemberInfo role);

		CollectionSemantic GetCollectionSemantic(MemberInfo role);
	}
}