using System;
using Iesi.Collections.Generic;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public enum PrinterType
	{
		Cd4C,
		Cd5C
	}

	public enum PaperCostType
	{
		Weight,
		Sheet
	}

	[Flags]
	public enum CostOptions
	{
		Plasticization = 1,
		UVPainting = 1 << 1,
		CreasingDie = 1 << 2,
		ConfTaglio = 1 << 3,
		ConfPM = 1 << 4,
		ConfBrossura = 1 << 5,
		Transport = 1 << 6,
		Packaging = 1 << 7,
		Various = 1 << 8
	}

	public class ProductQuotation : VersionModelBase
	{
		protected ProductQuotation()
			: this(null, null)
		{
		}

		public ProductQuotation(Quotation quotation, string name)
			: this(quotation, name, null)
		{
		}

		public ProductQuotation(Quotation quotation, string name, string group)
		{
			Quotation = quotation;
			Name = name;
			Group = group;
			Editions = new HashedSet<EditionQuotation>();
		}

		public virtual Quotation Quotation { get; protected set; }
		public virtual string Name { get; set; }
		public virtual string Group { get; set; }
		public virtual string Description { get; set; }
		public virtual string Format { get; set; }
		public virtual string Color { get; set; }
		public virtual PrinterType PrinterType{get; set; }
		public virtual string PaperType { get; set; }
		public virtual string PaperWeight { get; set; }
		public virtual PaperCostType PaperCostType{get; set; }
		public virtual decimal PaperUnitCost { get; set; }
		public virtual decimal PaperReamWeight { get; set; }
		public virtual decimal GraphicCost { get; set; }
		public virtual decimal PhotolithographyEquipmentCost { get; set; }
		public virtual CostOptions CostOptions{get; set; }
		public virtual ISet<EditionQuotation> Editions { get; protected set; }
	}
}