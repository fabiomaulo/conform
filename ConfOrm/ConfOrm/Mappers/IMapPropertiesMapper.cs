using System;

namespace ConfOrm.Mappers
{
	public interface IMapPropertiesMapper : ICollectionPropertiesMapper
	{
		void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping);
	}

	public interface IMapPropertiesMapper<TEntity, TKey, TElement> : ICollectionPropertiesMapper<TEntity, TElement> where TEntity : class
	{
		void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping);
	}
}