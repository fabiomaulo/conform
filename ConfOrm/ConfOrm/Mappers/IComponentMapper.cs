using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentAttributesMapper
	{
		void Parent(MemberInfo parent);
		void Parent(MemberInfo parent, Action<IParentMapper> parentMapping);		
	}

	public interface IComponentMapper : IComponentAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface IComponentAttributesMapper<TComponent>
	{
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class;
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IParentMapper> parentMapping) where TProperty : class;
	}

	public interface IComponentMapper<TComponent> : IComponentAttributesMapper<TComponent>, IPropertyContainerMapper<TComponent> where TComponent : class
	{
	}
}