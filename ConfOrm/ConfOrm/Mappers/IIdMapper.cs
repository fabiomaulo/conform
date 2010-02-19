using System;

namespace ConfOrm.Mappers
{
	public interface IIdMapper
	{
		void Generator(IGeneratorDef generator);
		void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping);
	}
}