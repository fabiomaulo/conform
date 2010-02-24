using System;

namespace ConfOrm.Mappers
{
	public interface IMapPropertiesMapper : ICollectionPropertiesMapper
	{
		void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping);
	}

	public interface IMapPropertiesMapper<TKey, TElement> : ICollectionPropertiesMapper<TElement>
	{
		void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping);
	}
}