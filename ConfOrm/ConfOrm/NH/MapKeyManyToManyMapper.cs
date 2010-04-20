using System;
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

		public void Column(Action<IColumnMapper> columnMapper)
		{
			throw new NotImplementedException();
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			throw new NotImplementedException();
		}

		public void Column(string name)
		{
			mapping.column = name;
		}
	}
}