using System.Linq;
using System.Reflection;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ConfOrm.Patterns
{
	public class SetCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of IPattern<MemberInfo>

		protected override bool MemberMatch(MemberInfo subject)
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