using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IClassAttributesMapper
	{
		void Id(Action<IIdMapper> idMapper);
		void Id(MemberInfo idProperty, Action<IIdMapper> idMapper);
		void Discriminator();		
	}

	public interface IClassMapper : IClassAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface IClassMapper<TEntity> : IClassMapper, IPropertyContainerMapper<TEntity> where TEntity : class
	{
		void Id<TProperty>(Expression<Func<TEntity, TProperty>> idProperty, Action<IIdMapper> idMapper);
	}
}