using System;

namespace ConfOrm.Mappers
{
	public interface IListPropertiesMapper : ICollectionPropertiesMapper
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}

	public interface IListPropertiesMapper<TElement> : ICollectionPropertiesMapper<TElement>
	{
		void Index(Action<IListIndexMapper> listIndexMapping);
	}
}