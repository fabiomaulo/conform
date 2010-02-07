using System;

namespace ConfOrm.Mappers
{
	public interface IMapPropertiesMapper : ICollectionPropertiesMapper
	{
		void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping);
	}
}