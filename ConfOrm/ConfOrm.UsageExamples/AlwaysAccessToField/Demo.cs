using System;
using System.Collections.Generic;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.Packs;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.AlwaysAccessToField
{
	public class Demo
	{
		[Test, Explicit]
		public void ApplyFieldAccessor_PropertyPerProperty_WherePossible()
		{
			// In this example I will compose various packs to customize the mapping.
			// There only one pack that is matter of the demo, the other are to have a right/nice mapping.
			// see also ConfOrm.UsageExamples.Packs.CompositionDemo

			var orm = new ObjectRelationalMapper();

			// The follow line show how compose patterns-appliers-packs and patterns-appliers
			IPatternsAppliersHolder patternsAppliers =
				(new AlwaysAccessToFieldWhereAvailablePack()) // <== in gerenal you have seen SafePropertyAccessorPack but this time I'm applying another pack
				.Merge(new NoPoidGuidApplier()) // only if you need Entities with neither a property nor a field representing the POID
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolTablesAndColumnsNamingPack(orm))
					.Merge(new TablePerClassPack());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			orm.TablePerClass<Person>();

			var mapping = mapper.CompileMappingFor(new[]{typeof(Person)});

			// In the out-put you will see that the accessor to the field will be applied only where possible
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void ApplyFieldAccessor_AsDefaultAccess()
		{
			// In this example I will compose various packs to customize the mapping.
			// There only one pack that is matter of the demo, the other are to have a right/nice mapping.
			// see also ConfOrm.UsageExamples.Packs.CompositionDemo

			var orm = new ObjectRelationalMapper();

			// The follow line show how compose patterns-appliers-packs and patterns-appliers
			IPatternsAppliersHolder patternsAppliers =
				(new AccessToPropertyWhereNoFieldPack()) // <== in this case I'll apply a default-access for the whole mapping, but there are cases where the field does not exists (let ConfORM check it ;) )
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolTablesAndColumnsNamingPack(orm))
					.Merge(new TablePerClassPack());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			orm.TablePerClass<Person>();

			var mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			mapping.defaultaccess = "field.camelcase"; // <== HERE I'm applying the default-access for all classes in the mapping of the whole domain

			// In the out-put you will see that the accessor is specified only where the field is not available
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void ApplyFieldAccessor_AsDefaultAccess_WithMappingDocumentForEachClass()
		{
			var orm = new ObjectRelationalMapper();
			IPatternsAppliersHolder patternsAppliers =
				(new AccessToPropertyWhereNoFieldPack()) // <== in this case I'll apply a default-access for the whole mapping, but there are cases where the field does not exists (let ConfORM check it ;) )
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolTablesAndColumnsNamingPack(orm))
					.Merge(new TablePerClassPack());

			var mapper = new Mapper(orm, patternsAppliers);

			orm.TablePerClass<Person>();

			var mappings = mapper.CompileMappingForEach(new[] { typeof(Person) }).WithDefaultAccessor("field.camelcase");

			foreach (var mapping in mappings)
			{
				Console.Write(mapping.AsString());
			}
		}

	}

	public static class HbmMappingExtensions
	{
		public static IEnumerable<HbmMapping> WithDefaultAccessor(this IEnumerable<HbmMapping> hbmMappings, string defaultAccessor)
		{
			foreach (var mapping in hbmMappings)
			{
				mapping.defaultaccess = "field.camelcase";
				yield return mapping;
			}
		}		
	}
}