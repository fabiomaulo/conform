using NHibernate.Mapping.ByCode;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	public class ComponentMultiUsagePropertyColumnNameApplier : ComponentMultiUsagePattern, IPatternApplier<PropertyPath, IPropertyMapper>
	{
		#region Implementation of IPatternApplier<PropertyPath,IPropertyMapper>

		public void Apply(PropertyPath subject, IPropertyMapper applyTo)
		{
			applyTo.Column(subject.ToColumnName());
		}

		#endregion
	}
}