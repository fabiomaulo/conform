using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ReadOnlyPropertyAccessorPattern : ReadOnlyPropertyPattern, IPatternApplier<MemberInfo, StateAccessStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,StateAccessStrategy>

		public StateAccessStrategy Apply(MemberInfo subject)
		{
			return StateAccessStrategy.ReadOnlyProperty;
		}

		#endregion
	}
}