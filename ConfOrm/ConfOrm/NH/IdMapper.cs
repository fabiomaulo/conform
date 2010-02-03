using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class IdMapper : IIdMapper
	{
		private readonly HbmId hbmId;

		public IdMapper(HbmId hbmId)
		{
			this.hbmId = hbmId;
			Generator(Generators.Native);
		}

		#region Implementation of IIdMapper

		public void Generator(Generators generator)
		{
			Generator(generator, x => { });
		}

		private void ApplyGenerator(Generators generator)
		{
			switch (generator)
			{
				case Generators.Native:
					new NativeGenerator(hbmId);
					break;
				case Generators.HighLow:
					new HighLowGenerator(hbmId);
					break;
				default:
					throw new ArgumentOutOfRangeException("generator");
			}
		}

		public void Generator(Generators generator, Action<IGeneratorMapper> generatorMapping)
		{
			ApplyGenerator(generator);
			generatorMapping(new GeneratorMapper(hbmId.generator));
		}

		#endregion
	}
}