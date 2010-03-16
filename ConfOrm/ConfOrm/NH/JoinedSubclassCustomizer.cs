using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class JoinedSubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IJoinedSubclassMapper<TEntity> where TEntity : class
	{
		public JoinedSubclassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) {}


		#region Implementation of IEntityAttributesMapper

		public void Key(Action<IKeyMapper> keyMapping)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Key(keyMapping));
		}

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.SelectBeforeUpdate(value));
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Loader(namedQueryReference));
		}

		public void SqlInsert(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.SqlInsert(sql));
		}

		public void SqlUpdate(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.SqlUpdate(sql));
		}

		public void SqlDelete(string sql)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.SqlDelete(sql));
		}

		#endregion

		#region Implementation of IJoinedSubclassAttributesMapper<TEntity>

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassMapper m) => m.Schema(schemaName));
		}

		#endregion
	}
}