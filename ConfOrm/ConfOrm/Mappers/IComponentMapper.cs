using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentAttributesMapper
	{
		void Parent(MemberInfo parent);
		void Parent(MemberInfo parent, Action<IComponentParentMapper> parentMapping);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
	}

	public interface IComponentMapper : IComponentAttributesMapper, IPropertyContainerMapper
	{
	}

	public interface IComponentAttributesMapper<TComponent>
	{
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class;
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class;
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Lazy(bool isLazy);
	}

	public interface IComponentMapper<TComponent> : IComponentAttributesMapper<TComponent>, IPropertyContainerMapper<TComponent> where TComponent : class
	{
	}
}