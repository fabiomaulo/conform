using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ClassMapper: AbstractPropertyContainerMapper, IClassMapper
	{
		private readonly HbmClass classMapping = new HbmClass();
		private readonly HbmId id;

		public ClassMapper(Type rootClass, HbmMapping mapDoc, MemberInfo idProperty)
			: base(rootClass, mapDoc)
		{
			var toAdd = new[] { classMapping };
			classMapping.name = rootClass.GetShortClassName(mapDoc);
			if(rootClass.IsAbstract)
			{
				classMapping.@abstract = true;
				classMapping.abstractSpecified = true;
			}
			if (idProperty != null)
			{
				var idType = GetMemberType(idProperty);
				id = new HbmId {name = idProperty.Name, type1 = idType.GetNhTypeName()};
				classMapping.Item = id;
			}
			else
			{
				id = new HbmId();
				classMapping.Item = id;				
			}
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

		#region Implementation of IClassMapper

		public void Id(Action<IIdMapper> idMapper)
		{
			idMapper(new IdMapper(id));
		}

		public void Id(MemberInfo idProperty, Action<IIdMapper> idMapper)
		{
			var idType = GetMemberType(idProperty);
			id.name = idProperty.Name;
			id.type1 = idType.GetNhTypeName();
			idMapper(new IdMapper(id));
		}

		public void Discriminator()
		{
			classMapping.discriminator = new HbmDiscriminator();
		}

		#endregion
	}
}