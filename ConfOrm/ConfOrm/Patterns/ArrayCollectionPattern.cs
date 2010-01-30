using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ArrayCollectionPattern : IPattern<MemberInfo>
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
			return memberType.IsArray;
		}

		#endregion
	}
}