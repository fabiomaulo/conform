using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class UnionSubclassCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IUnionSubclassMapper<TEntity> where TEntity : class
	{
		public UnionSubclassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder) {}

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			throw new NotImplementedException();
		}

		public void Proxy(Type proxy)
		{
			throw new NotImplementedException();
		}

		public void Lazy(bool value)
		{
			throw new NotImplementedException();
		}

		public void DynamicUpdate(bool value)
		{
			throw new NotImplementedException();
		}

		public void DynamicInsert(bool value)
		{
			throw new NotImplementedException();
		}

		public void BatchSize(int value)
		{
			throw new NotImplementedException();
		}

		public void SelectBeforeUpdate(bool value)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			throw new NotImplementedException();
		}

		public void SqlInsert(string sql)
		{
			throw new NotImplementedException();
		}

		public void SqlUpdate(string sql)
		{
			throw new NotImplementedException();
		}

		public void SqlDelete(string sql)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IUnionSubclassAttributesMapper<TEntity>

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IUnionSubclassAttributesMapper m) => m.Schema(schemaName));
		}

		#endregion
	}
}