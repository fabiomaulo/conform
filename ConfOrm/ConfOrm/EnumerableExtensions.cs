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

		public static bool IsSingle<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				return false;
			}
			var list = source as IList<TSource>;
			if (list != null)
			{
				return list.Count != 1;
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return false;
				}
				if (!enumerator.MoveNext())
				{
					return true;
				}
			}
			return false; ;
		}
	}
}