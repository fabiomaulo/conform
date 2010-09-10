using System;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class ManyToManyPluralizedTableApplier : ManyToManyInCollectionTableApplier
	{
		private readonly IInflector inflector;

		public ManyToManyPluralizedTableApplier(IDomainInspector domainInspector, IInflector inflector)
			: base(domainInspector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		protected override string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", inflector.Pluralize(names[0]), inflector.Pluralize(names[1]));
		}
	}
}