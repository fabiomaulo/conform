namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public abstract class CalculatedCost : Cost
	{
		public virtual decimal UnitCost { get; set; }

		public virtual int Quantity { get; protected set; }

		public override decimal Value
		{
			get
			{
				return UnitCost*Quantity;
			}
		}
	}
}