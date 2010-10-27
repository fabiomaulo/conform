using System;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class Demo
	{
		/*
		 * In this demo I'll show how create a custom column-naming pack to substitue some pattern-appliers of others packs.
		 * The trick is done using the 'UnionWith' extension instead 'Merge'.
		 */

		[Test, Explicit]
		public void ExclusionDemo()
		{
			var orm = new ObjectRelationalMapper();

			IPatternsAppliersHolder patternsAppliers =
				(new CoolPatternsAppliersHolder(orm))
					.UnionWith(new MyTablesAndColumnsNamingPack()); // <=== Note the usage of UnionWith instead Merge

			var mapper = new Mapper(orm, patternsAppliers);

			// The strategy to represent classes is not important
			orm.TablePerClass<PorBoxEO>();
			orm.TablePerClass<PorEO>();

			// Show the mapping to the console
			HbmMapping mapping =
				mapper.CompileMappingFor(new[] { typeof(PorEO), typeof(PorBoxEO) });
			Console.Write(mapping.AsString());
		}
	}
}