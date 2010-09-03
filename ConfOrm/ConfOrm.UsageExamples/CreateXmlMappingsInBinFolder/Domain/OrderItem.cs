namespace ConfOrm.UsageExamples.CreateXmlMappingsInBinFolder.Domain
{
	public class OrderItem
	{
		public virtual long Id { get; set; }
		public virtual int Quantity { get; set; }
		public virtual int ProductId { get; set; }
		public virtual Order Order { get; set; }
	}
}
