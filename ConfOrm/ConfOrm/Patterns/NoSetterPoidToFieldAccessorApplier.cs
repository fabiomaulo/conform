using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class NoSetterPoidToFieldAccessorApplier: NoSetterPropertyToFieldPattern, IPatternApplier<MemberInfo, IIdMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, IIdMapper applyTo)
		{
			applyTo.Access(Accessor.NoSetter);
		}

		#endregion
	}
}