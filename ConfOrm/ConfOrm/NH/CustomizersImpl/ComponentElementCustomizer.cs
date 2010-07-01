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

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			customizersHolder.AddCustomizer(typeof(TComponent), (IComponentAttributesMapper x) => x.Parent(member, parentMapping));
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property, Action<IComponentElementMapper<TNestedComponent>> mapping) where TNestedComponent : class
		{
			throw new NotImplementedException();
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			throw new NotImplementedException();
		}
	}
}