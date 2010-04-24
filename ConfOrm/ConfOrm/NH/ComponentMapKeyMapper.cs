using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ComponentMapKeyMapper : IComponentMapKeyMapper
	{
		private readonly HbmCompositeMapKey compositeMapKeyMapping;

		public ComponentMapKeyMapper(HbmCompositeMapKey compositeMapKeyMapping)
		{
			this.compositeMapKeyMapping = compositeMapKeyMapping;
		}

		public HbmCompositeMapKey CompositeMapKeyMapping
		{
			get { return compositeMapKeyMapping; }
		}

		public void Property(MemberInfo property, Action<IKeyPropertyMapper> mapping)
		{
			
		}

		public void ManyToOne(MemberInfo property, Action<IKeyManyToOneMapper> mapping)
		{
			
		}
	}
}