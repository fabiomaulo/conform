using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class DelegatedMemberApplier<TApplyTo> : IPatternApplier<MemberInfo, TApplyTo>
	{
		private readonly Action<TApplyTo> applier;
		private readonly Predicate<MemberInfo> matcher;

		public DelegatedMemberApplier(Predicate<MemberInfo> matcher, Action<TApplyTo> applier)
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

		#region Implementation of IPatternApplier<MemberInfo,TApplyTo>

		public void Apply(MemberInfo subject, TApplyTo applyTo)
		{
			applier(applyTo);
		}

		#endregion
	}
}