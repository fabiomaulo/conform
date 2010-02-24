namespace ConfOrm.Mappers
{
	public interface ISubclassAttributesMapper
	{
		
	}
	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
		
	}

	public interface ISubclassAttributesMapper<TEntity> where TEntity : class
	{

	}

	public interface ISubclassMapper<TEntity> : ISubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}