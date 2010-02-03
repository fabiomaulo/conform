using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	#region Nested type: AbstractGenerator

	public abstract class AbstractGenerator
	{
		protected AbstractGenerator(HbmId idMapping)
		{
			IdMapping = idMapping;
		}

		public HbmId IdMapping { get; private set; }
	}

	#endregion

	#region Nested type: NativeGenerator

	public class NativeGenerator : AbstractGenerator, IGenerator
	{
		public NativeGenerator(HbmId idMapping) : base(idMapping)
		{
			IdMapping.generator = new HbmGenerator { @class = "native" };
		}
	}

	#endregion

	public class HighLowGenerator : AbstractGenerator, IGenerator
	{
		public HighLowGenerator(HbmId idMapping) : base(idMapping)
		{
			IdMapping.generator = new HbmGenerator { @class = "hilo" };
		}
	}
}