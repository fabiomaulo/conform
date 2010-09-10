using System;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class UnionSubclassPluralizedTableApplier : InflectorNaming.UnionSubclassPluralizedTableApplier
	{
		public UnionSubclassPluralizedTableApplier(IInflector inflector) : base(inflector)
		{
		}

		protected override string GetTableName(Type subject)
		{
			return base.GetTableName(subject).ToUpperInvariant();
		}
	}
}