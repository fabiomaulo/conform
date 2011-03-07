using System;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.InflectorNaming
{
	public class ManyToManyPluralizedTableApplier : AbstractManyToManyInCollectionTableApplier
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

		public IInflector Inflector
		{
			get { return inflector; }
		}

		public override string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", inflector.Pluralize(names[0]), inflector.Pluralize(names[1]));
		}

		public override string GetTableNameForRelationOnProperty(string masterMany, string slaveMany, string propertyNameOnMaster)
		{
			throw new NotImplementedException();
		}
	}
}