namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public abstract class Cost : VersionModelBase
	{
		public virtual decimal Value { get; protected set; }
	}
}