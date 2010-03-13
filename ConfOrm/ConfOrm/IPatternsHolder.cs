using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public interface IPatternsHolder
	{
		ICollection<IPattern<MemberInfo>> Poids { get; }
		ICollection<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> PoidStrategies { get; }

		ICollection<IPattern<Type>> Componets { get; }

		ICollection<IPattern<MemberInfo>> Sets { get; }
		ICollection<IPattern<MemberInfo>> Bags { get; }
		ICollection<IPattern<MemberInfo>> Lists { get; }
		ICollection<IPattern<MemberInfo>> Arrays { get; }
		ICollection<IPattern<MemberInfo>> Dictionaries { get; }

		ICollection<IPatternValueGetter<Relation, Cascade?>> Cascades { get; }
		ICollection<IPattern<MemberInfo>> PersistentPropertiesExclusions { get; }

		ICollection<IPattern<Relation>> ManyToOneRelations { get; }
	}
}