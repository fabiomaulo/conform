using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.Packs;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.Packs
{
	public class CompositionDemo
	{
		[Test, Explicit]
		public void ComposingPatternsAppliersPacks()
		{
			// In this example I will compose various packs to customize the mapping.
			// The result is the same of ConfOrm.UsageExamples.Packs.SimpleDemo but this time using patterns-appliers-packs composition (instead its short-cut CoolPatternsAppliersHolder).
			// To play with patterns-appliers-packs-composition you need a more complex domain; adding and removing packs you can see
			// how change your mapping.

			// What is a patterns-appliers-pack:
			// It is an implementation of IPatternsAppliersHolder focused in a specific concern; 
			// for example CoolTablesAndColumnsNamingPack is focused to set columns and table names.

			var orm = new ObjectRelationalMapper();

			// The follow line show how compose patterns-appliers-packs and patterns-appliers
			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolTablesAndColumnsNamingPack(orm))
					.Merge(new TablePerClassPack())
					.Merge(new DatePropertyByNameApplier())
					.Merge(new MsSQL2008DateTimeApplier());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			// Note: I'm declaring the strategy only for the base entity
			orm.TablePerClassHierarchy<Animal>();

			// Note : I have to create mappings for the whole domain
			var mapping = mapper.CompileMappingFor(typeof(Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void ComposingPatternsAppliersPacksUsingInflector()
		{
			// In this example I'll use the same domain, but composing others packs, changing the strategy for the hierarchy

			var orm = new ObjectRelationalMapper();

			// The follow line show how compose patterns-appliers-packs and patterns-appliers
			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolColumnsNamingPack(orm))
					.Merge(new TablePerClassPack())
					.Merge(new PluralizedTablesPack(orm, new EnglishInflector()))
					.Merge(new ListIndexAsPropertyPosColumnNameApplier())
					.Merge(new DatePropertyByNameApplier())
					.Merge(new MsSQL2008DateTimeApplier());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			// Note: I'm declaring the strategy only for the base entity
			orm.TablePerClass<Animal>();

			// Note : I have to create mappings for the whole domain
			var mapping = mapper.CompileMappingFor(typeof(Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
			Console.Write(mapping.AsString());
		}
	}
}