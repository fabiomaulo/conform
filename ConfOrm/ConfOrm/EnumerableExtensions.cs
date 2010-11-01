using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm
{
	internal static class EnumerableExtensions
	{
		public static bool ContainsMember(this ICollection<MemberInfo> source, MemberInfo item)
		{
			return source.Count > 0 && (source.Contains(item) || (!item.DeclaringType.Equals(item.ReflectedType) && source.Contains(item.GetMemberFromDeclaringType())) ||
			                            item.GetPropertyFromInterfaces().Any(source.Contains));
		}
	}
}