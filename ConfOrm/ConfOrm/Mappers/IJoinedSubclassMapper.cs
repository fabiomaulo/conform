namespace ConfOrm.Mappers
{
	public interface IJoinedSubclassAttributesMapper
	{

	}
	public interface IJoinedSubclassMapper : IJoinedSubclassAttributesMapper,IPropertyContainerMapper
	{
		
	}

	public interface IJoinedSubclassAttributesMapper<TEntity> where TEntity : class
	{

	}

	public interface IJoinedSubclassMapper<TEntity> : IJoinedSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}

}