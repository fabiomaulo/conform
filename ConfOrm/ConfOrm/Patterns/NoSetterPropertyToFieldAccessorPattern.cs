using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class NoSetterPropertyToFieldAccessorPattern : IPatternApplier<MemberInfo, StateAccessStrategy>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if (property == null || property.CanWrite || !property.CanRead)
			{
				return false;
			}
			var fieldPattern = PropertyToFieldPatterns.Defaults.FirstOrDefault(pp => pp.Match(property));
			if (fieldPattern != null)
			{
				var fieldInfo = fieldPattern.GetBackFieldInfo(property);
				return fieldInfo.FieldType == property.PropertyType;
			}

			return false;
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,StateAccessStrategy>

		public StateAccessStrategy Apply(MemberInfo subject)
		{
			return StateAccessStrategy.FieldOnSet;
		}

		#endregion
	}
}