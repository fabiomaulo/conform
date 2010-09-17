namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public class ExplicitCost: Cost
	{
		public virtual void SetValue(decimal costValue)
		{
			Value = costValue;
		}
	}
}