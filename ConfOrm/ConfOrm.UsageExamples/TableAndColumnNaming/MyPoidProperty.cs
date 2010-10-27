using System.Reflection;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class MyPoidProperty : IPattern<MemberInfo>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var name = subject.Name;
			string rootEntityName = subject.DeclaringType.Name;
			if (rootEntityName.EndsWith("EO"))
			{
				rootEntityName = rootEntityName.Substring(0, rootEntityName.Length - 2);
			}
			var expected = rootEntityName + "Id";
			return name.Equals(expected);
		}

		#endregion
	}
}