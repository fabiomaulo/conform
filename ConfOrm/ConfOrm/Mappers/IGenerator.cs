using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.Mappers
{
	public interface IGenerator { }

	public static class Generators
	{
		static Generators()
		{
			Native = new NativeGenerator();
			HighLow = new HighLowGenerator();
		}

		public static IGenerator Native { get; private set; }
		public static IGenerator HighLow { get; private set; }

		#region Nested type: AbstractGenerator

		public abstract class AbstractGenerator
		{
			public abstract HbmGenerator GetCompiledGenerator();
		}

		#endregion

		#region Nested type: NativeGenerator

		private class NativeGenerator : AbstractGenerator, IGenerator
		{
			public override HbmGenerator GetCompiledGenerator()
			{
				return new HbmGenerator { @class = "native" };
			}
		}

		#endregion

		private class HighLowGenerator : AbstractGenerator, IGenerator
		{
			public override HbmGenerator GetCompiledGenerator()
			{
				return new HbmGenerator { @class = "hilo" };
			}
		}

	}
}