using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class AnyToFieldAccessorApplier : PropertyToFieldPattern, IPatternApplier<MemberInfo, IAnyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IAnyMapper>

		public void Apply(MemberInfo subject, IAnyMapper applyTo)
		{
			applyTo.Access(Accessor.Field);
		}

		#endregion
	}
}