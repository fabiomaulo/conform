using System;

namespace ConfOrm.Mappers
{
	public interface IJoinedSubclassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Key(Action<IKeyMapper> keyMapping);
		void Extends(Type baseType);
	}

	public interface IJoinedSubclassMapper : IJoinedSubclassAttributesMapper,IPropertyContainerMapper
	{
	}

	public interface IJoinedSubclassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
	}

	public interface IJoinedSubclassMapper<TEntity> : IJoinedSubclassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
		void Key(Action<IKeyMapper> keyMapping);
	}

}