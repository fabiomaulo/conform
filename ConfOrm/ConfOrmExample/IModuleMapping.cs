using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.NH;

namespace ConfOrmExample
{
	/// <summary>
	/// The module mapping.
	/// </summary>
	public interface IModuleMapping
	{
		/// <summary>
		/// Add the module's ORM definition.
		/// </summary>
		/// <param name="orm">The orm. </param>
		void DomainDefinition(ObjectRelationalMapper orm);

		/// <summary>
		/// The register patterns-appliers of the module.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="domainInspector">The domain inspector.</param>
		void RegisterPatterns(Mapper mapper, IDomainInspector domainInspector);

		/// <summary>
		/// The customize.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		void Customize(Mapper mapper);

		/// <summary>
		/// The get entities.
		/// </summary>
		/// <returns>
		/// All entities, to map, for the specific module.
		/// </returns>
		IEnumerable<Type> GetEntities();
	}

	public static class ModuleMappingUtil
	{
		public static IEnumerable<Type> RunModuleMapping<T>(ObjectRelationalMapper orm, Mapper mapper)
			where T : IModuleMapping, new()
		{
			IModuleMapping domainMapping = new T();
			domainMapping.DomainDefinition(orm);
			domainMapping.RegisterPatterns(mapper, orm);
			domainMapping.Customize(mapper);
			return domainMapping.GetEntities();
		}
	}
}