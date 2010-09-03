using System.Collections.Generic;

namespace ConfOrm.UsageExamples.CreateXmlMappingsInBinFolder.Domain
{
	public class Order
	{
		public virtual long Id { get; set; }
		public virtual string OrderNumber { get; set; }
		public virtual string CompanyName { get; set; }
		public virtual string Street { get; set; }
		public virtual string PostalCode { get; set; }
		public virtual string City { get; set; }
		public virtual IList<OrderItem> Items {get;set;}
	}
}
