using System.Linq;
using System.Reflection;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class PoidByAttributePattern: IPattern<MemberInfo>
	{
		public bool Match(MemberInfo subject)
		{
			return subject.GetCustomAttributes(typeof(IdAttribute), true).Any();
		}
	}
}