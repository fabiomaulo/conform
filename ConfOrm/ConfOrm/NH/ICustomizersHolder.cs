using System;
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

		void AddCustomizer(PropertyPath member, Action<IPropertyMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IManyToOneMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IOneToOneMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IAnyMapper> propertyCustomizer);

		void AddCustomizer(PropertyPath member, Action<ISetPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IBagPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IListPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IMapPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<ICollectionPropertiesMapper> propertyCustomizer);
		
		#region Collection Element relations

		//void AddCustomizer(PropertyPath member, Action<IManyToManyMapper> collectionRelationManyToManyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IElementMapper> collectionRelationElementCustomizer);
		//void AddCustomizer(PropertyPath member, Action<IOneToManyMapper> collectionRelationOneToManyCustomizer);

		#endregion

		#region Dictionary key relations

		//void AddCustomizer(PropertyPath member, Action<IMapKeyManyToManyMapper> mapKeyManyToManyCustomizer);
		//void AddCustomizer(PropertyPath member, Action<IMapKeyMapper> mapKeyElementCustomizer);

		#endregion

		void InvokeCustomizers(Type type, IClassAttributesMapper mapper);
		void InvokeCustomizers(Type type, ISubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IJoinedSubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IUnionSubclassAttributesMapper mapper);
		void InvokeCustomizers(Type type, IComponentMapper mapper);

		void InvokeCustomizers(PropertyPath member, IPropertyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IManyToOneMapper mapper);
		void InvokeCustomizers(PropertyPath member, IOneToOneMapper mapper);
		void InvokeCustomizers(PropertyPath member, IAnyMapper mapper);

		void InvokeCustomizers(PropertyPath member, ISetPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IBagPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IListPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IMapPropertiesMapper mapper);

		#region Collection Element relations invokers

		//void InvokeCustomizers(PropertyPath member, IManyToManyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IElementMapper mapper);
		//void InvokeCustomizers(PropertyPath member, IOneToManyMapper mapper);

		#endregion

		#region Dictionary key relations

		//void InvokeCustomizers(PropertyPath member, IMapKeyManyToManyMapper mapper);
		//void InvokeCustomizers(PropertyPath member, IMapKeyMapper mapper);

		#endregion
	}
}