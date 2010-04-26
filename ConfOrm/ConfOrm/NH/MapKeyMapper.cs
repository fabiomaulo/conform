using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class MapKeyMapper: IMapKeyMapper
	{
		private readonly HbmMapKey hbmMapKey;

		public MapKeyMapper(HbmMapKey hbmMapKey)
		{
			this.hbmMapKey = hbmMapKey;
		}

		public HbmMapKey MapKeyMapping
		{
			get { return hbmMapKey; }
		}

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
			hbmMapKey.column = name;
		}

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				hbmMapKey.type = persistentType.Name;
			}
		}

		public void Type<TPersistentType>() where TPersistentType : IUserType
		{
			Type(typeof(TPersistentType));
		}

		public void Type(Type persistentType)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof(IUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType");
			}
			hbmMapKey.type = persistentType.AssemblyQualifiedName;
		}

		public void Length(int length)
		{
			hbmMapKey.length = length.ToString();
		}
	}
}