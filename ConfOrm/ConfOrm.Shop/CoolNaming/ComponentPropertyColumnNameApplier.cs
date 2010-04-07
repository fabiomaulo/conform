using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ComponentPropertyColumnNameApplier : ComponentMemberDeepPathPattern, IPatternApplier<PropertyPath, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<PropertyPath,IPropertyMapper>

		public void Apply(PropertyPath subject, IPropertyMapper applyTo)
		{
			applyTo.Column(subject.ToColumnName());
		}

		#endregion
	}
}