namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public abstract class GenericModelBase<TId>
	{
		public virtual TId Id { get; protected set; }
	}
}