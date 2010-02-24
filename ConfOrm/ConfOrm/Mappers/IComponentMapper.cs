using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentMapper : IPropertyContainerMapper
	{
		void Parent(MemberInfo parent);
	}

	public interface IComponentMapper<TComponent> : IPropertyContainerMapper<TComponent> where TComponent : class
	{
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class;
	}

}