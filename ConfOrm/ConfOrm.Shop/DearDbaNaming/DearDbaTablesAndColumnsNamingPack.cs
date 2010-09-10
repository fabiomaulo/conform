using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class DearDbaTablesAndColumnsNamingPack : EmptyPatternsAppliersHolder
	{
		public DearDbaTablesAndColumnsNamingPack(IDomainInspector domainInspector, IInflector inflector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>
			            	{
			            		new ClassPluralizedTableApplier(inflector),
											new PoidColumnNameApplier()
			            	};
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>
			                 	{
			                 		new JoinedSubclassPluralizedTableApplier(inflector),
													new JoinedSubclassKeyAsRootIdColumnApplier(domainInspector)
			                 	};
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>
			                	{
			                		new UnionSubclassPluralizedTableApplier(inflector)
			                	};
			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>
			                 	{
			                 		new ManyToManyPluralizedTableApplier(domainInspector, inflector)
			                 	};
		}
	}
}