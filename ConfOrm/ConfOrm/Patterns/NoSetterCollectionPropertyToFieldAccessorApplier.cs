using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class NoSetterCollectionPropertyToFieldAccessorApplier : NoSetterPropertyToFieldPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPropertyMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Access(Accessor.NoSetter);
		}

		#endregion
	}
}