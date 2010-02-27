using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class ComponentCustomizer<TComponent> : PropertyContainerCustomizer<TComponent>, IComponentMapper<TComponent>
		where TComponent : class
	{
		public ComponentCustomizer(ICustomizersHolder customizersHolder, PropertyPath propertyPath) : base(customizersHolder, propertyPath) { }

		#region Implementation of IComponentMapper<TComponent>

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(parent);
			CustomizersHolder.AddCustomizer(typeof (TComponent), m => m.Parent(member));
		}

		#endregion
	}
}