using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.NamingAppliers;

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
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>
			               {
			               	new ComponentPropertyColumnNameApplier(),
			               };
		}
	}
}