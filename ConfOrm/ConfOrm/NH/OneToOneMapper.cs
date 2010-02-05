using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class OneToOneMapper: IOneToOneMapper
	{
		private readonly HbmOneToOne oneToOne;

		public OneToOneMapper(HbmOneToOne oneToOne)
		{
			this.oneToOne = oneToOne;
		}

		#region Implementation of IOneToOneMapper

		public void Cascade(Cascade cascadeStyle)
		{
			oneToOne.cascade = (cascadeStyle & Extensions.EachButDeleteOrphans).ToCascadeString();
		}

		#endregion
	}
}