using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoidGuidPattern : IPattern<MemberInfo>
	{
		public bool Match(MemberInfo subject)
		{
			var propertyOrFieldType = subject.GetPropertyOrFieldType();
			return propertyOrFieldType == typeof(Guid);
		}
	}
}