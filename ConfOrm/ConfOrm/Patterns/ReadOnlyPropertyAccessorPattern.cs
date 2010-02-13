using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ReadOnlyPropertyAccessorPattern : ReadOnlyPropertyPattern, IPatternValueGetter<MemberInfo, StateAccessStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,StateAccessStrategy>

		public StateAccessStrategy Get(MemberInfo element)
		{
			return StateAccessStrategy.ReadOnlyProperty;
		}

		#endregion
	}
}