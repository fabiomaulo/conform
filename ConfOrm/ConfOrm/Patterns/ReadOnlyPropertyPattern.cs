using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ReadOnlyPropertyPattern: IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if(property == null)
			{
				return false;
			}
			if (!property.CanWrite && property.CanRead)
			{
				return !PropertyToFieldPatterns.Defaults.Any(p=> p.Match(property));
			}
			return false;
		}

		#endregion
	}
}