using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class MapKeyManyToManyMapper : IMapKeyManyToManyMapper
	{
		private readonly HbmMapKeyManyToMany mapping;

		public MapKeyManyToManyMapper(HbmMapKeyManyToMany mapping)
		{
			this.mapping = mapping;
		}

		public HbmMapKeyManyToMany MapKeyManyToManyMapping
		{
			get { return mapping; }
		}

		#region Implementation of IMapKeyManyToManyMapper

		public void ForeignKey(string foreignKeyName)
		{
			mapping.foreignkey = foreignKeyName;
		}

		#endregion
	}
}