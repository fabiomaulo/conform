using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public interface IExplicitDeclarationsHolder
	{
		ICollection<Type> RootEntities { get; }
		ICollection<Type> TablePerClassEntities { get; }
		ICollection<Type> TablePerClassHierarchyEntities { get; }
		ICollection<Type> TablePerConcreteClassEntities { get; }
		ICollection<Relation> ManyToOneRelations { get; }
		ICollection<Relation> OneToManyRelations { get; }
		ICollection<Relation> OneToOneRelations { get; }
		ICollection<Relation> ManyToManyRelations { get; }
		ICollection<MemberInfo> Sets { get; }
		ICollection<MemberInfo> Bags { get; }
		ICollection<MemberInfo> Lists { get; }
		ICollection<MemberInfo> Arrays { get; }
		ICollection<MemberInfo> Dictionaries { get; }
		ICollection<Type> Components { get; }
		IDictionary<Relation, Cascade> Cascades { get; }
		ICollection<MemberInfo> PersistentProperties { get; }
		ICollection<Type> ComplexTypes { get; }
		ICollection<MemberInfo> Poids { get; }
	}
}