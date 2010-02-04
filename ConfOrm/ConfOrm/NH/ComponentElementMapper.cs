using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentElementMapper : IComponentElementMapper
	{
		private readonly HbmCompositeElement component;
		private readonly Type componentType;
		private readonly HbmMapping mapDoc;

		public ComponentElementMapper(Type componentType, HbmMapping mapDoc, HbmCompositeElement component)
		{
			this.componentType = componentType;
			this.mapDoc = mapDoc;
			this.component = component;
		}

		#region Implementation of IComponentElementMapper

		public void Property(MemberInfo property)
		{
			var hbmProperty = new HbmProperty { name = property.Name };
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

		public void ManyToOne(MemberInfo property)
		{
			var hbm = new HbmManyToOne { name = property.Name };
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
}