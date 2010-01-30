using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ConfOrm.Patterns
{
	public class SetCollectionPattern : IPattern<MemberInfo>
	{
		private static readonly List<AbstractPropertyToFieldPattern> PropertyToFieldPatterns =
			new List<AbstractPropertyToFieldPattern>
				{
					new PropertyToFieldCamelCasePattern(),
					new PropertyToFieldUnderscorePascalCasePattern(),
					new PropertyToFieldMUnderscorePascalCasePattern(),
					new PropertyToFieldUnderscoreCamelCasePattern()
				};

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (MemberMatch(subject)) return true;
			var pi = subject as PropertyInfo;
			if(pi != null)
			{
				var fieldPattern = PropertyToFieldPatterns.FirstOrDefault(pp => pp.Match(pi));
				if(fieldPattern != null)
				{
					var fieldInfo = fieldPattern.GetBackFieldInfo(pi);
					return MemberMatch(fieldInfo);
				}
			}
			return false;
		}

		private static bool MemberMatch(MemberInfo subject)
		{
				var memberType = subject.GetPropertyOrFieldType();
			if (typeof (ISet).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				var interfaces =
					memberType.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).ToList();
				if (memberType.IsInterface)
				{
					interfaces.Add(memberType.GetGenericTypeDefinition());
				}
				return interfaces.Contains(typeof (ISet<>));
			}
			return false;
		}

		#endregion
	}
}