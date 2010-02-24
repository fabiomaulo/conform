using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class CustomizersHolder : ICustomizersHolder
	{
		private readonly Dictionary<MemberInfo, List<Action<IBagPropertiesMapper>>> bagCustomizers =
			new Dictionary<MemberInfo, List<Action<IBagPropertiesMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<ICollectionPropertiesMapper>>> collectionCustomizers =
			new Dictionary<MemberInfo, List<Action<ICollectionPropertiesMapper>>>();

		private readonly Dictionary<Type, List<Action<IComponentMapper>>> componetClassCustomizers =
			new Dictionary<Type, List<Action<IComponentMapper>>>();

		private readonly Dictionary<Type, List<Action<IJoinedSubclassAttributesMapper>>> joinedClassCustomizers =
			new Dictionary<Type, List<Action<IJoinedSubclassAttributesMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<IListPropertiesMapper>>> listCustomizers =
			new Dictionary<MemberInfo, List<Action<IListPropertiesMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<IManyToOneMapper>>> manyToOneCustomizers =
			new Dictionary<MemberInfo, List<Action<IManyToOneMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<IMapPropertiesMapper>>> mapCustomizers =
			new Dictionary<MemberInfo, List<Action<IMapPropertiesMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<IOneToOneMapper>>> oneToOneCustomizers =
			new Dictionary<MemberInfo, List<Action<IOneToOneMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<IPropertyMapper>>> propertyCustomizers =
			new Dictionary<MemberInfo, List<Action<IPropertyMapper>>>();

		private readonly Dictionary<Type, List<Action<IClassAttributesMapper>>> rootClassCustomizers =
			new Dictionary<Type, List<Action<IClassAttributesMapper>>>();

		private readonly Dictionary<MemberInfo, List<Action<ISetPropertiesMapper>>> setCustomizers =
			new Dictionary<MemberInfo, List<Action<ISetPropertiesMapper>>>();

		private readonly Dictionary<Type, List<Action<ISubclassAttributesMapper>>> subclassCustomizers =
			new Dictionary<Type, List<Action<ISubclassAttributesMapper>>>();

		private readonly Dictionary<Type, List<Action<IUnionSubclassAttributesMapper>>> unionClassCustomizers =
			new Dictionary<Type, List<Action<IUnionSubclassAttributesMapper>>>();

		#region ICustomizersHolder Members

		public void AddCustomizer(Type type, Action<IClassAttributesMapper> classCustomizer)
		{
			AddCustomizer(rootClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<ISubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(subclassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IJoinedSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(joinedClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IUnionSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(unionClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(Type type, Action<IComponentMapper> classCustomizer)
		{
			AddCustomizer(componetClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IPropertyMapper> propertyCustomizer)
		{
			AddCustomizer(propertyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IManyToOneMapper> propertyCustomizer)
		{
			AddCustomizer(manyToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IOneToOneMapper> propertyCustomizer)
		{
			AddCustomizer(oneToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<ISetPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(setCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IBagPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(bagCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IListPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(listCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<IMapPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(mapCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(MemberInfo member, Action<ICollectionPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(collectionCustomizers, member, propertyCustomizer);
		}

		public void InvokeCustomizers(Type type, IClassAttributesMapper mapper)
		{
			InvokeCustomizers(rootClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, ISubclassAttributesMapper mapper)
		{
			InvokeCustomizers(subclassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IJoinedSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(joinedClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IUnionSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(unionClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(Type type, IComponentMapper mapper)
		{
			InvokeCustomizers(componetClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IPropertyMapper mapper)
		{
			InvokeCustomizers(propertyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IManyToOneMapper mapper)
		{
			InvokeCustomizers(manyToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IOneToOneMapper mapper)
		{
			InvokeCustomizers(oneToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, ISetPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(setCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IBagPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(bagCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IListPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(listCustomizers, member, mapper);
		}

		public void InvokeCustomizers(MemberInfo member, IMapPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(mapCustomizers, member, mapper);
		}

		#endregion

		private void AddCustomizer<TSubject, TCustomizable>(IDictionary<TSubject, List<Action<TCustomizable>>> customizers,
		                                                    TSubject member, Action<TCustomizable> customizer)
		{
			List<Action<TCustomizable>> actions;
			if (!customizers.TryGetValue(member, out actions))
			{
				actions = new List<Action<TCustomizable>>();
				customizers[member] = actions;
			}
			actions.Add(customizer);
		}

		private void InvokeCustomizers<TSubject, TCustomizable>(
			IDictionary<TSubject, List<Action<TCustomizable>>> customizers, TSubject member, TCustomizable customizable)
		{
			List<Action<TCustomizable>> actions;
			if (customizers.TryGetValue(member, out actions))
			{
				foreach (var action in actions)
				{
					action(customizable);
				}
			}
		}
	}
}