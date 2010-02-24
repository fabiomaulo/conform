using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class ClassCustomizer<TEntity>: PropertyContainerCustomizer<TEntity>, IClassMapper<TEntity> where TEntity : class
	{
		public ClassCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder)
		{}

		#region Implementation of IClassAttributesMapper<TEntity>

		public void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(idProperty);
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Id(member, idMapper));
		}

		#endregion

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.EntityName(value));
		}

		public void Proxy(Type proxy)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Proxy(proxy));
		}

		public void Lazy(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.Lazy(value));
		}

		public void DynamicUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.DynamicUpdate(value));
		}

		public void DynamicInsert(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.DynamicInsert(value));
		}

		public void BatchSize(int value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.BatchSize(value));
		}

		public void SelectBeforeUpdate(bool value)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), m => m.SelectBeforeUpdate(value));
		}

		#endregion
	}
}