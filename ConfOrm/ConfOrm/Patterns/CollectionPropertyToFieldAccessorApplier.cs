using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class CollectionPropertyToFieldAccessorApplier : PropertyToFieldPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Access(Accessor.Field);
		}

		#endregion
	}
}