using System;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class SubclassMapper : AbstractPropertyContainerMapper, ISubclassMapper
	{
		private readonly HbmSubclass classMapping = new HbmSubclass();

		public SubclassMapper(Type subClass, HbmMapping mapDoc) : base(subClass, mapDoc)
		{
			var toAdd = new[] { classMapping };
			classMapping.name = subClass.GetShortClassName(mapDoc);
			classMapping.extends = subClass.BaseType.GetShortClassName(mapDoc);
			mapDoc.Items = mapDoc.Items == null ? toAdd : mapDoc.Items.Concat(toAdd).ToArray();
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			classMapping.Items = classMapping.Items == null ? toAdd : classMapping.Items.Concat(toAdd).ToArray();
		}

		#endregion
	}
}