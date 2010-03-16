using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface ICustomizersHolder
	{
		void AddCustomizer(Type type, Action<IClassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<ISubclassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IJoinedSubclassMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IUnionSubclassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IComponentMapper> classCustomizer);

		void AddCustomizer(PropertyPath member, Action<IPropertyMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IManyToOneMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IOneToOneMapper> propertyCustomizer);

		void AddCustomizer(PropertyPath member, Action<ISetPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IBagPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IListPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IMapPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<ICollectionPropertiesMapper> propertyCustomizer);

		void InvokeCustomizers(Type type, IClassAttributesMapper mapper);
		void InvokeCustomizers(Type type, ISubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IJoinedSubclassMapper mapper);
		void InvokeCustomizers(Type type, IUnionSubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IComponentMapper mapper);

		void InvokeCustomizers(PropertyPath member, IPropertyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IManyToOneMapper mapper);
		void InvokeCustomizers(PropertyPath member, IOneToOneMapper mapper);

		void InvokeCustomizers(PropertyPath member, ISetPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IBagPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IListPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IMapPropertiesMapper mapper);
	}
}