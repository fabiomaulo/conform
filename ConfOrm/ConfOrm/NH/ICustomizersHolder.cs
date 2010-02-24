using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public interface ICustomizersHolder
	{
		void AddCustomizer(Type type, Action<IClassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<ISubclassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IJoinedSubclassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IUnionSubclassAttributesMapper> classCustomizer);
		void AddCustomizer(Type type, Action<IComponentMapper> classCustomizer);

		void AddCustomizer(MemberInfo member, Action<IPropertyMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<IManyToOneMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<IOneToOneMapper> propertyCustomizer);
		
		void AddCustomizer(MemberInfo member, Action<ISetPropertiesMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<IBagPropertiesMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<IListPropertiesMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<IMapPropertiesMapper> propertyCustomizer);
		void AddCustomizer(MemberInfo member, Action<ICollectionPropertiesMapper> propertyCustomizer);

		void InvokeCustomizers(Type type, IClassAttributesMapper mapper);
		void InvokeCustomizers(Type type, ISubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IJoinedSubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IUnionSubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IComponentMapper mapper);

		void InvokeCustomizers(MemberInfo member, IPropertyMapper mapper);
		void InvokeCustomizers(MemberInfo member, IManyToOneMapper mapper);
		void InvokeCustomizers(MemberInfo member, IOneToOneMapper mapper);

		void InvokeCustomizers(MemberInfo member, ISetPropertiesMapper mapper);
		void InvokeCustomizers(MemberInfo member, IBagPropertiesMapper mapper);
		void InvokeCustomizers(MemberInfo member, IListPropertiesMapper mapper);
		void InvokeCustomizers(MemberInfo member, IMapPropertiesMapper mapper);
	}
}