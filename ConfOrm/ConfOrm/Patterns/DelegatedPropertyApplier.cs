using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class DelegatedPropertyApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		private readonly Predicate<MemberInfo> matcher;
		private readonly Action<IPropertyMapper> applier;

		public DelegatedPropertyApplier(Predicate<MemberInfo> matcher, Action<IPropertyMapper> applier)
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

		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applier(applyTo);
		}

		#endregion
	}
}