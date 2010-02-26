namespace ConfOrm.Mappers
{
	public interface ISubclassAttributesMapper: IEntityAttributesMapper, IEntitySqlsMapper
	{
		void DiscriminatorValue(object value);
	}

	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface ISubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void DiscriminatorValue(object value);
	}

	public interface ISubclassMapper<TEntity> : ISubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}