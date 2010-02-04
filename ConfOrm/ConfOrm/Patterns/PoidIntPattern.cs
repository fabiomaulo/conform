using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoidIntPattern : IPattern<MemberInfo>
	{
		public bool Match(MemberInfo subject)
		{
			var propertyOrFieldType = subject.GetPropertyOrFieldType();
			return propertyOrFieldType == typeof (int) || propertyOrFieldType == typeof (long);
		}
	}
}