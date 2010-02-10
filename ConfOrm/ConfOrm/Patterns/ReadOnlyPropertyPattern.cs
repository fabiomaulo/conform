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
			MethodInfo[] accessors = property.GetAccessors();
			if (accessors == null)
			{
				return false;
			}
			if (accessors.FirstOrDefault(x => x.Name.StartsWith("set_")) == null
					&& accessors.FirstOrDefault(x => x.Name.StartsWith("get_")) != null)
			{
				return !PropertyToFieldPatterns.Defaults.Any(p=> p.Match(property));
			}
			return false;
		}

		#endregion
	}
}