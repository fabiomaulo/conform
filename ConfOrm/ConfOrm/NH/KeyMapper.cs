using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class KeyMapper: IKeyMapper
	{
		private readonly HbmKey mapping;

		public KeyMapper(Type ownerEntityType, HbmKey mapping)
		{
			this.mapping = mapping;
			this.mapping.column1 = ownerEntityType.Name.ToLowerInvariant() + "_key";
		}
	}
}