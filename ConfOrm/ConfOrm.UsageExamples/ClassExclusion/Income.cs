namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class Income : Movement<IncomeDetail>
	{
		public virtual string Origin { get; set; }
		public virtual string DispatchNumber { get; set; }
		public virtual string InvoiceNumber { get; set; }

		protected override IncomeDetail CreateNewDetail()
		{
			return new IncomeDetail(this);
		}

		protected override void ResetParentMovement(IncomeDetail detail)
		{
			detail.Movement = null;
		}
	}
}