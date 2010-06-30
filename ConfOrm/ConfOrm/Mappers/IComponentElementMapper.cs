using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentElementMapper
	{
		void Parent(MemberInfo parent);
		void Parent(MemberInfo parent, Action<IParentMapper> parentMapping);

		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentElementMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
	}

	public interface IComponentElementMapper<TComponent>
	{
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class;
		void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IParentMapper> parentMapping) where TProperty : class;

		void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping);

		void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property,
																		 Action<IComponentElementMapper<TNestedComponent>> mapping)
			where TNestedComponent : class;

		void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class;
	}
}