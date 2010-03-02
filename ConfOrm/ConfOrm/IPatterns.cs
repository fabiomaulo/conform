using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public interface IPatterns
	{
		ICollection<IPattern<MemberInfo>> PoidPatterns { get; }
		ICollection<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> PoidStrategyPatterns { get; }

		ICollection<IPattern<Type>> ComponetPatterns { get; }

		ICollection<IPattern<MemberInfo>> SetPatterns { get; }
		ICollection<IPattern<MemberInfo>> BagPatterns { get; }
		ICollection<IPattern<MemberInfo>> ListPatterns { get; }
		ICollection<IPattern<MemberInfo>> ArrayPatterns { get; }
		ICollection<IPattern<MemberInfo>> DictionaryPatterns { get; }

		ICollection<IPatternValueGetter<Relation, Cascade>> CascadePatterns { get; }
		ICollection<IPattern<MemberInfo>> PersistentPropertyExclusionPatterns { get; }
	}
}