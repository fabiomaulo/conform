using System;
using System.Reflection;

namespace ConfOrm
{
	public interface IDomainInspector
	{
		bool IsRootEntity(Type type);
		bool IsComponent(Type type);
		bool IsEntity(Type type);

		bool IsTablePerClass(Type type);
		bool IsTablePerClassHierarchy(Type type);
		bool IsTablePerConcreteClass(Type type);

		bool IsOneToOne(Type from, Type to);
		bool IsMasterOneToOne(Type from, Type to);
		bool IsManyToOne(Type from, Type to);
		bool IsManyToMany(Type role1, Type role2);
		bool IsMasterManyToMany(Type from, Type to);
		bool IsOneToMany(Type from, Type to);
		bool IsHeterogeneousAssociations(MemberInfo member);
		Cascade? ApplyCascade(Type from, MemberInfo on, Type to);

		bool IsPersistentId(MemberInfo member);
		IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member);

		bool IsPersistentProperty(MemberInfo role);

		bool IsSet(MemberInfo role);
		bool IsBag(MemberInfo role);
		bool IsList(MemberInfo role);
		bool IsArray(MemberInfo role);
		bool IsDictionary(MemberInfo role);
		bool IsComplex(MemberInfo member);
		bool IsVersion(MemberInfo member);
	}
}