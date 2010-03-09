using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class DelegatedMemberAdvancedApplier<TApplyTo> : IPatternApplier<MemberInfo, TApplyTo>
	{
		private readonly Action<MemberInfo,TApplyTo> applier;
		private readonly Predicate<MemberInfo> matcher;

		public DelegatedMemberAdvancedApplier(Predicate<MemberInfo> matcher, Action<MemberInfo, TApplyTo> applier)
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
			applier(subject, applyTo);
		}

		#endregion
	}
}