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

		public override string GetTableNameForRelation(Relation fromRelation, Relation toRelation)
		{
			return string.Format("{0}_{1}", inflector.Pluralize(fromRelation.From.Name), inflector.Pluralize(fromRelation.To.Name)).ToUpperInvariant();
		}

		public override string GetTableNameForRelationOnProperty(RelationOn fromRelation, RelationOn toRelation)
		{
			throw new NotImplementedException();
		}
	}
}