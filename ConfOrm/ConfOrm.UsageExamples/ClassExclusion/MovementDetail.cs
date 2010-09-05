namespace ConfOrm.UsageExamples.ClassExclusion
{
	public abstract class MovementDetail : BaseEntity
	{
		public virtual string Product { get; set; }
		public virtual string Supplier { get; set; }
		public virtual int Quantity { get; set; }
		public virtual string Observation { get; set; }
	}

	public abstract class MovementDetail<TMovement> : MovementDetail
	{
		protected MovementDetail()
		{
		}

		protected MovementDetail(TMovement movement)
		{
			Movement = movement;
		}

		public virtual TMovement Movement { get; protected internal set; }
	}
}