namespace ConfOrm.Mappers
{
	public interface IJoinedSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{

	}
	public interface IJoinedSubclassMapper : IJoinedSubclassAttributesMapper,IPropertyContainerMapper
	{
		
	}

	public interface IJoinedSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{

	}

	public interface IJoinedSubclassMapper<TEntity> : IJoinedSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}

}