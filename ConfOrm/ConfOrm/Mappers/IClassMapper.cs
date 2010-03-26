using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IClassAttributesMapper : IEntityAttributesMapper, IEntitySqlsMapper
	{
		void Id(Action<IIdMapper> idMapper);
		void Id(MemberInfo idProperty, Action<IIdMapper> idMapper);
		void Discriminator();
		void DiscriminatorValue(object value);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Mutable(bool isMutable);
		void Version(MemberInfo versionProperty, Action<IVersionMapper> versionMapping);
		void NaturalId(Action<INaturalIdMapper> naturalIdMapping);
	}

	public interface IClassMapper : IClassAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface IClassAttributesMapper<TEntity> : IEntityAttributesMapper, IEntitySqlsMapper where TEntity : class
	{
		void Id(Action<IIdMapper> idMapper);
		void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper);
		void DiscriminatorValue(object value);
		void Table(string tableName);
		void Catalog(string catalogName);
		void Schema(string schemaName);
		void Mutable(bool isMutable);
		void Version<TProperty>(Expression<Func<TEntity, TProperty>> versionProperty, Action<IVersionMapper> versionMapping);
		void NaturalId(Action<INaturalIdMapper> naturalIdMapping);
	}

	public interface IClassMapper<TEntity> : IClassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}