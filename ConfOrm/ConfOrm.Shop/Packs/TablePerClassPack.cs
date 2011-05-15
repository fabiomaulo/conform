using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class TablePerClassPack : EmptyPatternsAppliersHolder
	{
		public TablePerClassPack()
		{
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> { new JoinedSubclassOnDeleteApplier() };
		}
	}
}