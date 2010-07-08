using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Subclassing;

namespace ConfOrm.Shop.Packs
{
	public partial class TablePerClassHierarchyDiscriminatorPack : EmptyPatternsAppliersHolder
	{
		public TablePerClassHierarchyDiscriminatorPack(IDomainInspector domainInspector)
		{
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>
			            	{
			            		new DiscriminatorColumnNameApplier(domainInspector),
											new ClassDiscriminatorValueAsClassNameApplier(domainInspector)
			            	};
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>
			           	{
			           		new SubclassDiscriminatorValueAsClassNameApplier()
			           	};
		}
	}
}