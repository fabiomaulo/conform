using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ClassMapper: AbstractPropertyContainerMapper, IClassMapper
	{
		private readonly HbmClass classMapping;

		public ClassMapper(Type rootClass, HbmMapping mapDoc, MemberInfo idProperty)
			: base(rootClass, mapDoc)
		{
			classMapping = new HbmClass();
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
				classMapping.Item = new HbmId {name = idProperty.Name, type1 = idType.GetNhTypeName()};
			}
			else
			{
				classMapping.Item = new HbmId();				
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
			idMapper(new IdMapper((HbmId)classMapping.Item));
		}

		public void Id(MemberInfo idProperty, Action<IIdMapper> idMapper)
		{
			var idType = GetMemberType(idProperty);
			var id = (HbmId)classMapping.Item;
			id.name = idProperty.Name;
			id.type1 = idType.GetNhTypeName();
			idMapper(new IdMapper(id));
		}

		public void Discriminator()
		{
			if (classMapping.discriminator == null)
			{
				classMapping.discriminator = new HbmDiscriminator();
			}
		}

		#endregion
	}
}