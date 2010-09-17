using System;
using Iesi.Collections.Generic;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public class Quotation : VersionModelBase
	{
		public Quotation()
		{
			Products = new HashedSet<ProductQuotation>();
			AgentPercentage = 1;
			CompanyPercentage = 1;
			CreatedOn = DateTime.Now;
			ValidUntil = CreatedOn.AddMonths(1);
		}

		public virtual DateTime CreatedOn { get; set; }
		public virtual string Code { get; set; }
		public virtual string Reference { get; set; }
		public virtual string Description { get; set; }
		public virtual DateTime ValidUntil { get; set; }
		public virtual string PaymentMethod { get; set; }
		public virtual string TransportMethod { get; set; }
		public virtual decimal AgentPercentage { get; set; }
		public virtual decimal CompanyPercentage { get; set; }
		public virtual ISet<ProductQuotation> Products { get; private set; }
	}
}