using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class RequiredPropertyPatternApplier: IPatternApplier<MemberInfo, IPropertyMapper>
	{
		private readonly IDomainInspector domainInspector;

		public RequiredPropertyPatternApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		public bool Match(MemberInfo subject)
		{
			var hasAttribute = subject.GetCustomAttributes(typeof(RequiredAttribute), true).Any();
			if(!hasAttribute)
			{
				return false;
			}
			return !domainInspector.IsTablePerClassHierarchy(subject.DeclaringType) || domainInspector.IsRootEntity(subject.DeclaringType);
		}

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.NotNullable(true);
		}
	}
}