using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentMapper : AbstractPropertyContainerMapper, IComponentMapper
	{
		private readonly HbmComponent component;

		public ComponentMapper(HbmComponent component, Type componentType, HbmMapping mapDoc) : base(componentType, mapDoc)
		{
			this.component = component;
			component.@class = componentType.GetShortClassName(mapDoc);
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

		#endregion

		#region Implementation of IComponentMapper

		public void Parent(MemberInfo parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			component.parent = new HbmParent {name = parent.Name};
		}

		#endregion
	}
}