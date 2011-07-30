using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace ConfOrm.NH.CustomizersImpl
{
	public class ManyToAnyCustomizer : IManyToAnyMapper
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public ManyToAnyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void MetaType(IType metaType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType(metaType));
		}

		public void MetaType<TMetaType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType<TMetaType>());
		}

		public void MetaType(System.Type metaType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType(metaType));
		}

		public void IdType(IType idType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType(idType));
		}

		public void IdType<TIdType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType<TIdType>());
		}

		public void IdType(System.Type idType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType(idType));
		}

		public void Columns(Action<IColumnMapper> idColumnMapping, Action<IColumnMapper> classColumnMapping)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.Columns(idColumnMapping, classColumnMapping));
		}

		public void MetaValue(object value, System.Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaValue(value, entityType));
		}
	}
}