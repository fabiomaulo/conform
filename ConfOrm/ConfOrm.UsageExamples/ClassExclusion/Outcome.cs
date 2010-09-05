namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class Outcome : Movement<OutcomeDetail>
	{
		public virtual string Branch { get; set; }

		protected override OutcomeDetail CreateNewDetail()
		{
			return new OutcomeDetail(this);
		}

		protected override void ResetParentMovement(OutcomeDetail detail)
		{
			detail.Movement = null;
		}
	}
}