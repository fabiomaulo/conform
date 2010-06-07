using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrm.Shop.Packs
{
	public class AutoAccessFieldPack : EmptyPatternsAppliersHolder
	{
		public AutoAccessFieldPack()
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new MemberNoSetterToFieldAccessorApplier<IIdMapper>(),
			       	};

			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			           	{
			           		new MemberReadOnlyAccessorApplier<IPropertyMapper>(),
			           		new MemberNoSetterToFieldAccessorApplier<IPropertyMapper>(),
			           		new MemberToFieldAccessorApplier<IPropertyMapper>(),
			           	};

			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			             	{
			             		new MemberReadOnlyAccessorApplier<ICollectionPropertiesMapper>(),
			             		new MemberNoSetterToFieldAccessorApplier<ICollectionPropertiesMapper>(),
			             		new MemberToFieldAccessorApplier<ICollectionPropertiesMapper>(),
			             	};

			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new MemberReadOnlyAccessorApplier<IManyToOneMapper>(),
			            		new MemberNoSetterToFieldAccessorApplier<IManyToOneMapper>(),
			            		new MemberToFieldAccessorApplier<IManyToOneMapper>(),
			            	};

			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new MemberReadOnlyAccessorApplier<IOneToOneMapper>(),
			           		new MemberNoSetterToFieldAccessorApplier<IOneToOneMapper>(),
			           		new MemberToFieldAccessorApplier<IOneToOneMapper>(),
			           	};

			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>
			      	{
			      		new MemberReadOnlyAccessorApplier<IAnyMapper>(),
			      		new MemberNoSetterToFieldAccessorApplier<IAnyMapper>(),
			      		new MemberToFieldAccessorApplier<IAnyMapper>()
			      	};
		}
	}
}