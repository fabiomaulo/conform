using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm
{
	public static class ForClass<T>
	{
		private const BindingFlags DefaultFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		public static MemberInfo Property(Expression<Func<T, object>> propertyGetter)
		{
			if (propertyGetter == null)
			{
				return null;
			}
			return TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
		}

		public static MemberInfo Field(string fieldName)
		{
			if (fieldName == null)
			{
				return null;
			}

			return typeof(T).GetField(fieldName, DefaultFlags);
		}
	}
}