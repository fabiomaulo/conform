using System;

namespace ConfOrm.Mappers
{
	public interface IIdMapper
	{
		void Generator(Generators generator);
		void Generator(Generators generator, Action<IGeneratorMapper> generatorMapping);
	}
}