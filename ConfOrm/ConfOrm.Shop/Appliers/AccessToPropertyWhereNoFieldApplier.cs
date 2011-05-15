using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Appliers
{
	public class AccessToPropertyWhereNoFieldApplier<TApplyTo> : IPatternApplier<MemberInfo, TApplyTo>
		where TApplyTo : IAccessorPropertyMapper
	{
		public bool Match(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if (property == null)
			{
				return false;
			}
			return !PropertyToFieldPatterns.Defaults.Any(pp => pp.Match(property));
		}

		public void Apply(MemberInfo subject, TApplyTo applyTo)
		{
			applyTo.Access(Accessor.Property);
		}
	}
}