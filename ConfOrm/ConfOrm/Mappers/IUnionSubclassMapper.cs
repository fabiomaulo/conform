namespace ConfOrm.Mappers
{
	public interface IUnionSubclassAttributesMapper
	{

	}
	public interface IUnionSubclassMapper : IUnionSubclassAttributesMapper, IPropertyContainerMapper
	{

	}

	public interface IUnionSubclassAttributesMapper<TEntity> where TEntity : class
	{

	}

	public interface IUnionSubclassMapper<TEntity> : IUnionSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}

}