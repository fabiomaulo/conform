using System;
using System.Linq.Expressions;
using ConfOrm.Mappers;
using NHibernate.UserTypes;

namespace ConfOrm.NH.CustomizersImpl
{
	public class CollectionPropertiesCustomizer<TEntity, TElement> : ICollectionPropertiesMapper<TEntity, TElement>
		where TEntity : class
	{
		private readonly IKeyMapper<TEntity> keyMapper;
		private readonly PropertyPath propertyPath;

		public CollectionPropertiesCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			CustomizersHolder = customizersHolder;
			keyMapper = new CollectionKeyCustomizer<TEntity>(propertyPath, customizersHolder);
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }

		#region Implementation of ICollectionPropertiesMapper<TEntity,TElement>

		public void Inverse(bool value)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x)=> x.Inverse(value));
		}

		public void Mutable(bool value)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Mutable(value));
		}

		public void Where(string sqlWhereClause)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Where(sqlWhereClause));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.BatchSize(value));
		}

		public void Lazy(CollectionLazy collectionLazy)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Lazy(collectionLazy));
		}

		public void Key(Action<IKeyMapper<TEntity>> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void OrderBy<TProperty>(Expression<Func<TElement, TProperty>> property)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.OrderBy(member));
		}

		public void Sort()
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Sort());
		}

		public void Sort<TComparer>()
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Sort<TComparer>());
		}

		public void Cascade(Cascade cascadeStyle)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Cascade(cascadeStyle));
		}

		public void Type<TCollection>() where TCollection : IUserCollectionType
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Type<TCollection>());
		}

		public void Type(Type collectionType)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Type(collectionType));
		}

		public void Table(string tableName)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Table(tableName));
		}

		public void Catalog(string catalogName)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Catalog(catalogName));
		}

		public void Schema(string schemaName)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Schema(schemaName));
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => x.Cache(cacheMapping));
		}

		#endregion
	}
}