using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PropertyToFieldAccessorApplier : PropertyToFieldPattern, IPatternApplier<MemberInfo, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.Access(Accessor.Field);
		}

		#endregion
	}
}