using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class ClassCustomizer<TEntity>: PropertyContainerCustomizer<TEntity>, IClassMapper<TEntity> where TEntity : class
	{
		public ClassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null)
		{}

		#region Implementation of IClassAttributesMapper<TEntity>

		public void Id(Action<IIdMapper> idMapper)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Id(idMapper));
		}

		public void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(idProperty);
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Id(member, idMapper));
		}

		public void DiscriminatorValue(object value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.DiscriminatorValue(value));
		}

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Schema(schemaName));
		}

		#endregion

		#region Implementation of IEntityAttributesMapper

		public void Mutable(bool isMutable)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Mutable(isMutable));
		}

		public void Version<TProperty>(Expression<Func<TEntity, TProperty>> versionProperty, Action<IVersionMapper> versionMapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(versionProperty);
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Version(member, versionMapping));
		}

		public void NaturalId(Action<INaturalIdMapper> naturalIdMapping)
		{
			throw new NotImplementedException();
		}

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.SelectBeforeUpdate(value));
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IClassAttributesMapper m) => m.SqlDelete(sql));
		}

		#endregion
	}
}