using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BagCollectionPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (MemberMatch(subject))
			{
				return true;
			}
			var pi = subject as PropertyInfo;
			if (pi != null)
			{
				AbstractPropertyToFieldPattern fieldPattern = PropertyToFieldPatterns.Defaults.FirstOrDefault(pp => pp.Match(pi));
				if (fieldPattern != null)
				{
					FieldInfo fieldInfo = fieldPattern.GetBackFieldInfo(pi);
					return MemberMatch(fieldInfo);
				}
			}
			return false;
		}

		private static bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			if (typeof(IEnumerable).IsAssignableFrom(memberType) && !IsNotSupportedAsBag(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				List<Type> interfaces =
					memberType.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).ToList();
				if (memberType.IsInterface)
				{
					interfaces.Add(memberType.GetGenericTypeDefinition());
				}
				return interfaces.Contains(typeof(IEnumerable<>));
			}
			return false;
		}

		#endregion

		private static bool IsNotSupportedAsBag(Type memberType)
		{
			return memberType == typeof(string);
		}
	}
}