namespace ConfOrm.Mappers
{
	public interface IJoinedSubclassMapper : IPropertyContainerMapper
	{
		
	}

	public interface IJoinedSubclassMapper<TEntity> : IJoinedSubclassMapper, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}