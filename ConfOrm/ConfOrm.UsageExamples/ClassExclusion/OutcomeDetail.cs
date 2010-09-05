namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class OutcomeDetail : MovementDetail<Outcome>
	{
		protected OutcomeDetail()
		{
		}

		public OutcomeDetail(Outcome movement)
			: base(movement)
		{
		}
	}
}