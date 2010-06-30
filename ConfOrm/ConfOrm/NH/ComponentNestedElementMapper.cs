using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentNestedElementMapper : IComponentElementMapper
	{
		private readonly HbmNestedCompositeElement component;
		private readonly Type componentType;
		protected readonly HbmMapping mapDoc;
		private IParentMapper parentMapper;

		public ComponentNestedElementMapper(Type componentType, HbmMapping mapDoc, HbmNestedCompositeElement component)
		{
			this.componentType = componentType;
			this.mapDoc = mapDoc;
			this.component = component;
		}

		#region Implementation of IComponentElementMapper

		public void Parent(MemberInfo parent)
		{
			Parent(parent, x => { });
		}

		public void Parent(MemberInfo parent, Action<IParentMapper> parentMapping)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			var mapper = GetParentMapper(parent);
			parentMapping(mapper);
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
			mapping(new ManyToOneMapper(property, hbm, mapDoc));
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

		private IParentMapper GetParentMapper(MemberInfo parent)
		{
			if (parentMapper != null)
			{
				return parentMapper;
			}
			component.parent = new HbmParent();
			return parentMapper = new ParentMapper(component.parent, parent);
		}
	}
}