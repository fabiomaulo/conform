using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ComponentElementCustomizer<TComponent> : IComponentElementMapper<TComponent>
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public ComponentElementCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Parent(member));
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IComponentParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Parent(member, parentMapping));
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, memberOf), mapping);
		}

		public void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property, Action<IComponentElementMapper<TNestedComponent>> mapping) where TNestedComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(new PropertyPath(propertyPath, member), customizersHolder));
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			mapping(new ComponentElementCustomizer<TNestedComponent>(new PropertyPath(propertyPath, memberOf), customizersHolder));
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, memberOf), mapping);
		}
	}
}