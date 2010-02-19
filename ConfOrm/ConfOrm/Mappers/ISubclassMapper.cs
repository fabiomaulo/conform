namespace ConfOrm.Mappers
{
	public interface ISubclassAttributesMapper
	{
		
	}
	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
		
	}

	public interface ISubclassMapper<TEntity> : ISubclassMapper, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}