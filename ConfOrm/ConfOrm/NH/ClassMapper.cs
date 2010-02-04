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
			classMapping.name = rootClass.AssemblyQualifiedName;
			if(rootClass.IsAbstract)
			{
				classMapping.@abstract = true;
				classMapping.abstractSpecified = true;
			}
			var idType = GetMemberType(idProperty);
			id = new HbmId { name = idProperty.Name, type1 = idType.GetNhTypeName() };
			classMapping.Item = id;
			mapDoc.Items = mapDoc.Items == null ? toAdd : mapDoc.Items.Concat(toAdd).ToArray();
		}

		public HbmId Id
		{
			get { return id; }
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

		public void Discriminator()
		{
			classMapping.discriminator = new HbmDiscriminator();
		}

		#endregion
	}
}