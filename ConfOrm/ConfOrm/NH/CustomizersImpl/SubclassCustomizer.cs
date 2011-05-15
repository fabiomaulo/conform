using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Persister.Entity;

namespace ConfOrm.NH.CustomizersImpl
{
	public class SubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, ISubclassMapper<TEntity> where TEntity : class
	{
		public SubclassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) {}

		public void DiscriminatorValue(object value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.DiscriminatorValue(value));
		}

		#region Implementation of IEntityAttributesMapper

		public void Join(string splitGroupId, Action<IJoinMapper<TEntity>> splittedMapping)
		{
			throw new NotSupportedException();
		}

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.SelectBeforeUpdate(value));
		}

		public void Persister<T>() where T : IEntityPersister
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Persister<T>());
		}

		public void Synchronize(params string[] table)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Synchronize(table));
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.SqlDelete(sql));
		}

		public void Subselect(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (ISubclassMapper m) => m.Subselect(sql));
		}

		#endregion
	}
}