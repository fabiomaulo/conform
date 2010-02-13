namespace ConfOrm.Mappers
{
	public interface IUnionSubclassMapper : IPropertyContainerMapper
	{
		
	}

	public interface IUnionSubclassMapper<TEntity> : IUnionSubclassMapper, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}