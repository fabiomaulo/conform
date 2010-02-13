namespace ConfOrm.Mappers
{
	public interface ISubclassMapper : IPropertyContainerMapper
	{
		
	}

	public interface ISubclassMapper<TEntity> : ISubclassMapper, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}