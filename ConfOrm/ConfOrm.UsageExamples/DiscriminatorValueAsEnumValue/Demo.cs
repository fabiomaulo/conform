using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.Packs;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.DiscriminatorValueAsEnumValue
{
	public class Demo
	{
		// only for mapping scope (in the domain it is not needed) I have a enum to classify the hierarchy
		private enum ItemTypes
		{
			Item = -1,
			Post = 0,
			Contribute = 5,
			Page = 7,
			Gallery = 9,
		}

		[Test, Explicit]
		public void DiscriminatorValuePatternsAppliersPacksComposition()
		{
			// In this example I will compose various packs to customize the mapping.
			// In this case I concern on the two packs about Table-per-class-hierarchy discriminator value pattern
			// Note: both DiscriminatorValueAsClassNamePack and DiscriminatorValueAsEnumValuePack does contain DiscriminatorColumnNameApplier but with the Merge
			// it will be applied just once

			var orm = new ObjectRelationalMapper();

			// The follow line show how compose patterns-appliers-packs and patterns-appliers
			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm)) // <== NOTE: it can be used together with DiscriminatorValueAsEnumValuePack
					.Merge(new DiscriminatorValueAsEnumValuePack<Item, ItemTypes>(orm)) // <== NOTE: it can be used together with DiscriminatorValueAsClassNamePack
					.Merge(new CoolTablesAndColumnsNamingPack(orm))
					.Merge(new TablePerClassPack())
					.Merge(new DatePropertyByNameApplier())
					.Merge(new MsSQL2008DateTimeApplier());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			// Note: I'm declaring the strategy only for the base entities
			orm.TablePerClassHierarchy<Item>();
			orm.TablePerClassHierarchy<Something>();

			// Note : I have to create mappings for the whole domain
			var mapping = mapper.CompileMappingFor(typeof(Item).Assembly.GetTypes().Where(t => t.Namespace == typeof(Item).Namespace));
			Console.Write(mapping.AsString());
		}

	}
}