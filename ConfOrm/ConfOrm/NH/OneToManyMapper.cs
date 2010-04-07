using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class OneToManyMapper: IOneToManyMapper
	{
		private readonly Type collectionElementType;
		private readonly HbmOneToMany oneToManyMapping;

		public OneToManyMapper(Type collectionElementType, HbmOneToMany oneToManyMapping, HbmMapping mapDoc)
		{
			if (oneToManyMapping == null)
			{
				throw new ArgumentNullException("oneToManyMapping");
			}
			this.collectionElementType = collectionElementType;
			if(collectionElementType != null)
			{
				oneToManyMapping.@class = collectionElementType.GetShortClassName(mapDoc);
			}
			this.oneToManyMapping = oneToManyMapping;
		}

		#region Implementation of IOneToManyMapper

		public void NotFound(NotFoundMode mode)
		{
			if (mode == null)
			{
				return;
			}
			oneToManyMapping.notfound = mode.ToHbm();
		}

		#endregion
	}
}