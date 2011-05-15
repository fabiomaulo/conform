using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.Packs
{
	public class AccessToPropertyWhereNoFieldPack : EmptyPatternsAppliersHolder
	{
		public AccessToPropertyWhereNoFieldPack()
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new AccessToPropertyWhereNoFieldApplier<IIdMapper>(),
			       	};

			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			           	{
			           		new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>(),
			           	};

			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new AccessToPropertyWhereNoFieldApplier<ICollectionPropertiesMapper>(),
			             	};

			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new AccessToPropertyWhereNoFieldApplier<IManyToOneMapper>(),
			            	};

			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new AccessToPropertyWhereNoFieldApplier<IOneToOneMapper>(),
			           	};

			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>
			      	{
			      		new AccessToPropertyWhereNoFieldApplier<IAnyMapper>(),
			      	};
			componentParent = new List<IPatternApplier<MemberInfo, IComponentParentMapper>>
			                  	{
			                  		new AccessToPropertyWhereNoFieldApplier<IComponentParentMapper>(),
			                  	};
			componentProperty = new List<IPatternApplier<MemberInfo, IComponentAttributesMapper>>
			                    	{
			                    		new AccessToPropertyWhereNoFieldApplier<IComponentAttributesMapper>(),
			                    	};
		}
	}
}