using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class IdMapper : IIdMapper
	{
		private readonly HbmId hbmId;
		private IGenerator generator;

		public IdMapper(HbmId hbmId)
		{
			this.hbmId = hbmId;
			Generator = Generators.Native;
		}

		#region Implementation of IIdMapping

		public IGenerator Generator
		{
			get { return generator; }
			set
			{
				generator = value;
				hbmId.generator = generator != null ? ((Generators.AbstractGenerator)generator).GetCompiledGenerator() : null;
			}
		}

		#endregion
	}
}