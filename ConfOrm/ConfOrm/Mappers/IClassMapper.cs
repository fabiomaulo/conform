using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IClassAttributesMapper : IEntityAttributesMapper
	{
		void Id(Action<IIdMapper> idMapper);
		void Id(MemberInfo idProperty, Action<IIdMapper> idMapper);
		void Discriminator();
		void DiscriminatorValue(object value);
	}

	public interface IClassMapper : IClassAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface IClassAttributesMapper<TEntity> : IEntityAttributesMapper where TEntity : class
	{
		void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper);
	}

	public interface IClassMapper<TEntity> : IClassAttributesMapper<TEntity>, IPropertyContainerMapper<TEntity> where TEntity : class
	{
	}
}