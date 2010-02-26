namespace ConfOrm.Mappers
{
	public interface ISubclassAttributesMapper: IEntityAttributesMapper, IEntitySqlsMapper
	{
		
	}

	public interface ISubclassMapper : ISubclassAttributesMapper, IPropertyContainerMapper
	{
		void DiscriminatorValue(object value);
	}

	public interface ISubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void DiscriminatorValue(object value);
	}

	public interface ISubclassMapper<TEntity> : ISubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}