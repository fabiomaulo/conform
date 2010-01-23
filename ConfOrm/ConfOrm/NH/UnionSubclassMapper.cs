using System;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class UnionSubclassMapper : AbstractPropertyContainerMapper, IPropertyContainerMapper
	{
		private readonly HbmUnionSubclass classMapping = new HbmUnionSubclass();

		public UnionSubclassMapper(Type subClass, HbmMapping mapDoc)
			: base(subClass, mapDoc)
		{
			var toAdd = new[] { classMapping };
			classMapping.name = subClass.AssemblyQualifiedName;
			classMapping.extends = subClass.BaseType.AssemblyQualifiedName;
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