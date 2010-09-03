using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.Packs;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.Packs
{
	public class SimpleDemo
	{
		[Test, Explicit]
		public void AddAppliersPackToGivenCoolPatternsAppliersHolder()
		{
			// In this example I will add the DiscriminatorValueAsClassNamePack to customize the mapping con discriminator column name and values
			// for table-per-class-hierarchy mapping

			var orm = new ObjectRelationalMapper();

			// Merge of CoolPatternsAppliersHolder with DiscriminatorValueAsClassNamePack
			// IMPORTANT: In this case the Merge extension will return a new instance of IPatternsAppliersHolder with the result of the merge.
			var patternsAppliers = (new CoolPatternsAppliersHolder(orm)).Merge(new DiscriminatorValueAsClassNamePack(orm));

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			// Note: I'm declaring the strategy only for the base entity
			orm.TablePerClassHierarchy<Animal>();

			// Note : I have to create mappings for the whole domain
			var mapping = mapper.CompileMappingFor(typeof(Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void WithoutDiscriminatorValueAsClassNamePack()
		{
			// In this example show the result of the mapping but WITHOUT merge the CoolPatternsAppliersHolder with DiscriminatorValueAsClassNamePack
			// this mean that will be used the default behavior and column names defined in NHibernate

			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));

			orm.TablePerClassHierarchy<Animal>();

			var mapping = mapper.CompileMappingFor(typeof(Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
			Console.Write(mapping.AsString());
		}
	}
}