using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface IPatternsAppliersHolder
	{
		ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid { get; }
		ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property { get; }
		ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> ManyToOne { get; }
		ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection { get; }
		ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath { get; }
		ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> ManyToOnePath { get; }
		ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath { get; }
	}
}