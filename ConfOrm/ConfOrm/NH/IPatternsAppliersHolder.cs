using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface IPatternsAppliersHolder
	{
		IList<IPatternApplier<MemberInfo, IIdMapper>> Poid { get; }
		IList<IPatternApplier<MemberInfo, IPropertyMapper>> Property { get; }
		IList<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection { get; }
	}
}