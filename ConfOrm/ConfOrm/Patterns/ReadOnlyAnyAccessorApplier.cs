using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class ReadOnlyAnyAccessorApplier : ReadOnlyPropertyPattern, IPatternApplier<MemberInfo, IAnyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IAnyMapper>

		public void Apply(MemberInfo subject, IAnyMapper applyTo)
		{
			applyTo.Access(Accessor.ReadOnly);
		}

		#endregion
	}
}