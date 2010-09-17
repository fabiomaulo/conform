
using System;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public class QuantityCost : CalculatedCost
	{
		public virtual void SetQuantity(int value)
		{
			Quantity = value;
		}
	}
}