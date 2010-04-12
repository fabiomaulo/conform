using System;
using System.Reflection;

namespace ConfOrm.Shop.Patterns
{
	public class PoidPropertyAsClassNameId : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var name = subject.Name;
			var expected = subject.DeclaringType.Name + GetIdPostfix();
			return name.Equals(expected);
		}

		#endregion

		protected virtual string GetIdPostfix()
		{
			return "Id";
		}
	}
}