using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PoIdGuidStrategyPattern : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var pi = subject as PropertyInfo;
			if(pi!=null)
			{
				return pi.PropertyType == typeof (Guid);
			}
			return false;
		}

		#endregion
	}
}