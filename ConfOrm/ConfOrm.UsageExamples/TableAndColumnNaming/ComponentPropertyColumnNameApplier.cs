using NHibernate.Mapping.ByCode;
using ConfOrm.NH;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class ComponentPropertyColumnNameApplier : IPatternApplier<PropertyPath, IPropertyMapper>
	{
		public bool Match(PropertyPath subject)
		{
			// it match for any property (not only for components)
			return !ReferenceEquals(subject, null);
		}

		public void Apply(PropertyPath subject, IPropertyMapper applyTo)
		{
			applyTo.Column(subject.ToColumnName().Underscore());
		}
	}
}