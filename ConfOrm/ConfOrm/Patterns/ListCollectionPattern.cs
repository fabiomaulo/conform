using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ListCollectionPattern : AbstractCollectionPattern
	{
		#region Implementation of AbstractCollectionPattern

		protected override bool MemberMatch(MemberInfo subject)
		{
			Type memberType = subject.GetPropertyOrFieldType();
			if (typeof (IList).IsAssignableFrom(memberType))
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
				var isList = interfaces.Contains(typeof (IList<>));
				// a bidirectional one-to-many should use Bag or Set
				return isList && !(new BidirectionalOneToManyPattern().Match(new Relation(subject.DeclaringType, memberType.DetermineCollectionElementType())));
			}
			return false;
		}

		#endregion
	}
}