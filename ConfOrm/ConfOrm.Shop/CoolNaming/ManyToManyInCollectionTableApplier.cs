using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.NamingAppliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyInCollectionTableApplier : AbstractManyToManyInCollectionTableApplier
	{
		public ManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		protected override string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", names[0], names[1]);
		}
	}
}