using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class NoSetterPropertyToFieldAccessorApplier : NoSetterPropertyToFieldPattern, IPatternApplier<MemberInfo, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, IPropertyMapper applyTo)
		{
			applyTo.Access(Accessor.NoSetter);
		}

		#endregion
	}
}