using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentElementMapper : IComponentElementMapper
	{
		private readonly HbmCompositeElement component;
		private readonly Type componentType;
		protected readonly HbmMapping mapDoc;

		public ComponentElementMapper(Type componentType, HbmMapping mapDoc, HbmCompositeElement component)
		{
			this.componentType = componentType;
			this.mapDoc = mapDoc;
			this.component = component;
		}

		#region Implementation of IComponentElementMapper

		public void Parent(MemberInfo parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			component.parent = new HbmParent { name = parent.Name };
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmProperty { name = property.Name };
			mapping(new PropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public void Component(MemberInfo property, Action<IComponentElementMapper> mapping)
		{
			var nestedComponentType = property.GetPropertyOrFieldType();
			var hbm = new HbmNestedCompositeElement
			          	{name = property.Name, @class = nestedComponentType.GetShortClassName(mapDoc)};
			mapping(new ComponentNestedElementMapper(nestedComponentType, mapDoc, hbm));
			AddProperty(hbm);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(hbm));
			AddProperty(hbm);
		}

		#endregion

		protected void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}
	}

	public class ComponentElementMapper<TComponent> : ComponentElementMapper, IComponentElementMapper<TComponent> where TComponent : class
	{
		public ComponentElementMapper(HbmMapping mapDoc, HbmCompositeElement component) : base(typeof(TComponent), mapDoc, component) { }

		#region Implementation of IComponentElementMapper<TComponent>

		public void Parent<TProperty>(Expression<Func<TComponent, TProperty>> parent) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(parent);
			Parent(member);
		}

		public void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			Property(member, mapping);
		}

		public void Component<TNestedComponent>(Expression<Func<TComponent, TNestedComponent>> property, Action<IComponentElementMapper<TNestedComponent>> mapping) where TNestedComponent : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			var nestedComponentType = typeof(TNestedComponent);
			var hbm = new HbmNestedCompositeElement { name = member.Name, @class = nestedComponentType.GetShortClassName(mapDoc) };
			mapping(new ComponentNestedElementMapper<TNestedComponent>(mapDoc, hbm));
			AddProperty(hbm);
		}

		public void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			ManyToOne(member, mapping);
		}

		#endregion
	}

}