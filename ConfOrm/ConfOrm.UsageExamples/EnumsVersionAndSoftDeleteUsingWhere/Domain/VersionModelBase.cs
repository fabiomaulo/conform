using System;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public abstract class VersionModelBase : GenericModelBase<long>
	{
		public virtual long Version { get; set; }
		public virtual DateTime? DeletedOn { get; set; }
	}
}