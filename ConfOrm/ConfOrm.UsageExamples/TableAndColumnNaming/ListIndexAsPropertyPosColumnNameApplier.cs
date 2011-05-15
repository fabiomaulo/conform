using NHibernate.Mapping.ByCode;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class ListIndexAsPropertyPosColumnNameApplier : ConfOrm.Shop.CoolNaming.ListIndexAsPropertyPosColumnNameApplier
	{
		protected override string GetIndexColumnName(PropertyPath subject)
		{
			return base.GetIndexColumnName(subject).Underscore();
		}
	}
}