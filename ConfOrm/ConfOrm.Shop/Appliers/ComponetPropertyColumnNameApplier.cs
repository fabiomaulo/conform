using System;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.Appliers
{
	public class ComponetPropertyColumnNameApplier : ComponentMemberDeepPathPattern, IPatternApplier<PropertyPath, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<PropertyPath,IPropertyMapper>

		public void Apply(PropertyPath subject, IPropertyMapper applyTo)
		{
			applyTo.Column(subject.ToColumnName());
		}

		#endregion
	}
}