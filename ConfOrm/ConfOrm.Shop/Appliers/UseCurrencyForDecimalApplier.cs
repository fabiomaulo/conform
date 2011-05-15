using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate;

namespace ConfOrm.Shop.Appliers
{
	public class UseCurrencyForDecimalApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			return subject.GetPropertyOrFieldType() == typeof (decimal);
		}

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.Type(NHibernateUtil.Currency);
		}
	}
}