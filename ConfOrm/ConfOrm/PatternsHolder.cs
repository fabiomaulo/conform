using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	public class PatternsHolder : IPatternsHolder
	{
		private readonly List<IPattern<MemberInfo>> arrayPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> bagPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPatternValueGetter<Relation, Cascade>> cascadePatterns =
			new List<IPatternValueGetter<Relation, Cascade>>();

		private readonly List<IPattern<Type>> componetPatterns = new List<IPattern<Type>>();
		private readonly List<IPattern<MemberInfo>> dictionaryPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> listPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPattern<MemberInfo>> persistentPropertyExclusionPatterns = new List<IPattern<MemberInfo>>();
		private readonly List<IPattern<MemberInfo>> poidPatterns = new List<IPattern<MemberInfo>>();

		private readonly List<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> poidStrategyPatterns =
			new List<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>>();

		private readonly List<IPattern<MemberInfo>> setPatterns = new List<IPattern<MemberInfo>>();

		#region Implementation of IPatternsHolder

		public ICollection<IPattern<MemberInfo>> Poids
		{
			get { return poidPatterns; }
		}

		public ICollection<IPatternValueGetter<MemberInfo, IPersistentIdStrategy>> PoidStrategies
		{
			get { return poidStrategyPatterns; }
		}

		public ICollection<IPattern<Type>> Componets
		{
			get { return componetPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Sets
		{
			get { return setPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Bags
		{
			get { return bagPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Lists
		{
			get { return listPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Arrays
		{
			get { return arrayPatterns; }
		}

		public ICollection<IPattern<MemberInfo>> Dictionaries
		{
			get { return dictionaryPatterns; }
		}

		public ICollection<IPatternValueGetter<Relation, Cascade>> Cascades
		{
			get { return cascadePatterns; }
		}

		public ICollection<IPattern<MemberInfo>> PersistentPropertiesExclusions
		{
			get { return persistentPropertyExclusionPatterns; }
		}

		#endregion
	}
}