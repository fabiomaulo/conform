using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ManyToOneMapper: IManyToOneMapper
	{
		private readonly HbmManyToOne manyToOne;

		public ManyToOneMapper(HbmManyToOne manyToOne)
		{
			this.manyToOne = manyToOne;
		}

		#region Implementation of IManyToOneMapper

		public void Cascade(Cascade cascadeStyle)
		{
			manyToOne.cascade = (cascadeStyle & ~ConfOrm.Cascade.DeleteOrphans).ToCascadeString();
		}

		#endregion
	}
}