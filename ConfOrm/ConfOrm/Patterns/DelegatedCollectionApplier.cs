using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class DelegatedCollectionApplier : IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		private readonly Action<ICollectionPropertiesMapper> applier;
		private readonly Predicate<MemberInfo> matcher;

		public DelegatedCollectionApplier(Predicate<MemberInfo> matcher, Action<ICollectionPropertiesMapper> applier)
		{
			if (matcher == null)
			{
				throw new ArgumentNullException("matcher");
			}
			if (applier == null)
			{
				throw new ArgumentNullException("applier");
			}
			this.matcher = matcher;
			this.applier = applier;
		}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			return matcher(subject);
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applier(applyTo);
		}

		#endregion
	}
}