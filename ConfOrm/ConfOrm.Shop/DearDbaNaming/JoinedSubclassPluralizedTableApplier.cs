using System;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class JoinedSubclassPluralizedTableApplier : InflectorNaming.JoinedSubclassPluralizedTableApplier
	{
		public JoinedSubclassPluralizedTableApplier(IInflector inflector) : base(inflector)
		{
		}

		protected override string GetTableName(Type subject)
		{
			return base.GetTableName(subject).ToUpperInvariant();
		}
	}
}