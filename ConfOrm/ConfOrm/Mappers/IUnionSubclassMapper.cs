namespace ConfOrm.Mappers
{
	public interface IUnionSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{

	}
	public interface IUnionSubclassMapper : IUnionSubclassAttributesMapper, IPropertyContainerMapper
	{

	}

	public interface IUnionSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{

	}

	public interface IUnionSubclassMapper<TEntity> : IUnionSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}

}