using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BagCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
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