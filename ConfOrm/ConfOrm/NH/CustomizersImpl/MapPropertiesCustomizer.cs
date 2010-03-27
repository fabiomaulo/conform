using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class MapPropertiesCustomizer<TEntity, TKey, TElement> : CollectionPropertiesCustomizer<TEntity, TElement>, IMapPropertiesMapper<TEntity, TKey, TElement> where TEntity : class
	{
		public MapPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder) : base(propertyPath, customizersHolder) {}

		#region Implementation of IMapPropertiesMapper<TEntity,TKey,TElement>

		public void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping)
		{
			CustomizersHolder.AddCustomizer(PropertyPath, (IMapPropertiesMapper x) => x.MapKeyManyToMany(mapKeyMapping));
		}

		#endregion
	}
}