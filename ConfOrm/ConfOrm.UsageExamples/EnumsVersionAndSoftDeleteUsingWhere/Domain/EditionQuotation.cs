namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public class EditionQuotation : VersionModelBase
	{
		protected EditionQuotation()
		{
		}

		public EditionQuotation(ProductQuotation productQuotation, int copyCount)
		{
			ProductQuotation = productQuotation;
			CopyCount = copyCount;
			PlateCost = new QuantityCost();
			StartupCost = new QuantityCost();
			PrintCost = new QuantityCost();
			InkCost = new QuantityCost();
			PaperCost = new QuantityCost();
			PlasticizationCost = new ExplicitCost();
			UVPaintingCost = new ExplicitCost();
			CreasingDieCost = new ExplicitCost();
			ConfTaglioCost = new ExplicitCost();
			ConfPMCost = new ExplicitCost();
			ConfBrossuraCost = new ExplicitCost();
			TransportCost = new ExplicitCost();
			PackagingCost = new ExplicitCost();
			VariousCost = new ExplicitCost();
		}

		public virtual ProductQuotation ProductQuotation { get; protected set; }
		public virtual int CopyCount { get; private set; }
		public virtual QuantityCost PlateCost { get; private set; }
		public virtual QuantityCost StartupCost { get; private set; }
		public virtual QuantityCost PaperCost { get; private set; }
		public virtual CalculatedCost PrintCost { get; private set; }
		public virtual QuantityCost InkCost { get; private set; }
		public virtual ExplicitCost PlasticizationCost { get; private set; }
		public virtual ExplicitCost UVPaintingCost { get; private set; }
		public virtual ExplicitCost CreasingDieCost { get; private set; }
		public virtual ExplicitCost ConfTaglioCost { get; private set; }
		public virtual ExplicitCost ConfPMCost { get; private set; }
		public virtual ExplicitCost ConfBrossuraCost { get; private set; }
		public virtual ExplicitCost TransportCost { get; private set; }
		public virtual ExplicitCost PackagingCost { get; private set; }
		public virtual ExplicitCost VariousCost { get; private set; }
	}
}