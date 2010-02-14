using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface ICustomizersHolder
	{
		IDictionary<MemberInfo, Action<IPropertyMapper>> PropertyCustomizers { get; }
		IDictionary<MemberInfo, Action<IManyToOneMapper>> ManyToOneCustomizers { get; }
		IDictionary<MemberInfo, Action<IOneToOneMapper>> OneToOneCustomizers { get; }
		IDictionary<MemberInfo, Action<ICollectionPropertiesMapper>> CollectionCustomizers { get; }
	}
}