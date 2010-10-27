using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	/// <summary>
	/// In this pack I'll implements some (not some != all).
	/// What is very important is the fact that I'll use same class names used in the CoolTablesAndColumnsNamingPack then
	/// instead use the "Merge" extension to compose packs (<see cref="ConfOrm.UsageExamples.Packs.CompositionDemo"/>) I'll
	/// use the "Union" extension; in this way I can replace some already implemented pattern-appliers with my own.
	/// </summary>
	public class MyTablesAndColumnsNamingPack : EmptyPatternsAppliersHolder
	{
		public MyTablesAndColumnsNamingPack()
		{
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       {
			       	new PoidPropertyColumnNameApplier(),
			       };
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>
			               {
			               	new ComponentPropertyColumnNameApplier(),
			               };
		}
	}
}