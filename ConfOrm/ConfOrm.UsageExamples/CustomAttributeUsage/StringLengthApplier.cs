using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class StringLengthApplier : IPatternApplier<MemberInfo, IPropertyMapper>
	{
		public bool Match(MemberInfo subject)
		{
			return typeof(string).Equals(subject.GetPropertyOrFieldType()) && subject.GetCustomAttributes(typeof(StringLengthAttribute), true).Any();
		}

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			var attribute = (StringLengthAttribute)subject.GetCustomAttributes(typeof(StringLengthAttribute), true).First();
			applyTo.Length(attribute.MaximumLength);
		}
	}
}