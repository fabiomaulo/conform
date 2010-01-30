using System;
using System.Linq;
using System.Reflection;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ConfOrm.Patterns
{
	public class SetCollectionPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (MemberMatch(memberType)) return true;

			return false;
		}

		private bool MemberMatch(Type memberType)
		{
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