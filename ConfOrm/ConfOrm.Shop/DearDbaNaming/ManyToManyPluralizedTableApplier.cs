using System;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class ManyToManyPluralizedTableApplier: AbstractManyToManyInCollectionTableApplier
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

		public override string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}_{1}", inflector.Pluralize(names[0]), inflector.Pluralize(names[1])).ToUpperInvariant();
		}

		public override string GetTableNameForRelationOnProperty(string masterMany, string slaveMany, string propertyNameOnMaster)
		{
			throw new NotImplementedException();
		}
	}
}