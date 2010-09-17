using System;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain
{
	public class PiecesSpeedCost : CalculatedCost
	{
		public virtual int Pieces { get; set; }

		public virtual int Speed { get; set; }

		public override int Quantity
		{
			get { return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Pieces)/Convert.ToDouble(Speed))); }
		}
	}
}