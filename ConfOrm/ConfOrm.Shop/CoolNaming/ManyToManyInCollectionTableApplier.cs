using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyInCollectionTableApplier : AbstractManyToManyInCollectionTableApplier
	{
		public ManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public override string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", names[0], names[1]);
		}
	}
}