using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.Packs
{
	public class AlwaysAccessToFieldWhereAvailablePack : EmptyPatternsAppliersHolder
	{
		public AlwaysAccessToFieldWhereAvailablePack()
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new AlwaysAccessToFieldWhereAvailableApplier<IIdMapper>(),
			       	};

			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			           	{
			           		new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>(),
			           	};

			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new AlwaysAccessToFieldWhereAvailableApplier<ICollectionPropertiesMapper>(),
			             	};

			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new AlwaysAccessToFieldWhereAvailableApplier<IManyToOneMapper>(),
			            	};

			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new AlwaysAccessToFieldWhereAvailableApplier<IOneToOneMapper>(),
			           	};

			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>
			      	{
			      		new AlwaysAccessToFieldWhereAvailableApplier<IAnyMapper>(),
			      	};
			componentParent = new List<IPatternApplier<MemberInfo, IComponentParentMapper>>
			                  	{
			                  		new AlwaysAccessToFieldWhereAvailableApplier<IComponentParentMapper>(),
			                  	};
			componentProperty = new List<IPatternApplier<MemberInfo, IComponentAttributesMapper>>
			                    	{
			                    		new AlwaysAccessToFieldWhereAvailableApplier<IComponentAttributesMapper>(),
			                    	};
		}
	}
}