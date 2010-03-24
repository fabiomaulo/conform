using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm
{
	internal static class EnumerableExtensions
	{
		public static bool ContainsMember(this ICollection<MemberInfo> source, MemberInfo item)
		{
			return source.Contains(item) || (!item.DeclaringType.Equals(item.ReflectedType) && source.Contains(item.GetMemberFromDeclaringType()));
		}
	}
}