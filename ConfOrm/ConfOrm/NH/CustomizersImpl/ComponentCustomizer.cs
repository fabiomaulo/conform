using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IComponentMapper<TComponent>
		where TComponent : class
	{
		public ComponentCustomizer(ICustomizersHolder customizersHolder) : base(customizersHolder, null) { }
		public ComponentCustomizer(ICustomizersHolder customizersHolder, PropertyPath propertyPath) : base(customizersHolder, propertyPath) { }

		#region Implementation of IComponentMapper<TComponent>

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			Parent(parent, x=> { });
		}

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent, Action<IParentMapper> parentMapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			CustomizersHolder.AddCustomizer(typeof(TComponent), m => m.Parent(member, parentMapping));
		}

		#endregion
	}
}