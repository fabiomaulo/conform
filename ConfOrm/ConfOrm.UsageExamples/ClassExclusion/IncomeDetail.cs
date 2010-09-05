namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class IncomeDetail : MovementDetail<Income>
	{
		protected IncomeDetail()
		{
		}

		public IncomeDetail(Income movement)
			: base(movement)
		{
		}

		public virtual double Price { get; set; }
		public virtual double Tax { get; set; }
	}
}