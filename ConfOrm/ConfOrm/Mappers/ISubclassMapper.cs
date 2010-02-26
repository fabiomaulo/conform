namespace ConfOrm.Mappers
{
	public interface ISubclassAttributesMapper: IEntityAttributesMapper, IEntitySqlsMapper
	{
		
	}
	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
		
	}

	public interface ISubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{

	}

	public interface ISubclassMapper<TEntity> : ISubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}