using System;
using System.Collections.Generic;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.UsageExamples.CreateXmlMappingsInBinFolder.Domain;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.UsageExamples.CreateXmlMappingsInBinFolder
{
	/// <summary>
	/// Example of ConfORM initializer.
	/// </summary>
	/// <remarks>
	/// Where you don't have a big domain a class like this can be enough to organize your mappings.
	/// </remarks>
	public class ConfOrmInitializer
	{
		private Mapper mapper;
		private ObjectRelationalMapper orm;
		private bool isInitialized;
		private readonly Type[] tablePerClassEntities = new[] {typeof (Order), typeof (OrderItem)};

		public IEnumerable<HbmMapping> GetCompiledMappingsPerClass()
		{
			Initialize();
			return mapper.CompileMappingForEach(tablePerClassEntities);
		}

		private void Initialize()
		{
			if(isInitialized)
			{
				return;
			}
			isInitialized = true;

			orm = new ObjectRelationalMapper();
			mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));

			// The real mapping
			orm.TablePerClass(tablePerClassEntities);
		}
	}
}