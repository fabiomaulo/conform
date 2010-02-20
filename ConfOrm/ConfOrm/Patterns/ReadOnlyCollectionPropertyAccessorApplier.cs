using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class ReadOnlyCollectionPropertyAccessorApplier : ReadOnlyPropertyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Access(Accessor.ReadOnly);
		}

		#endregion
	}
}