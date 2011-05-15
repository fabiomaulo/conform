using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.Packs;
using ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere.Domain;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.EnumsVersionAndSoftDeleteUsingWhere
{
	public class Demo
	{
		[Test, Explicit]
		public void ComposingPatternsAppliersPacks()
		{
			// Thanks to Lorenzo Vegetti to provide the domain of this example.
			// This example refers to a little-domain, interesting from the point of view of mapping with ConfORM.
			// Here you can see how apply general convetion, how and when compose patterns-appliers-packs and patterns-appliers,
			// how ConfORM can apply different mapping depending on the type of enum (see the difference on CostOptions) and so on...
			// You don't have to organize the mapping all in one class, as in this example, and you don't have to use the IModuleMapping...
			// You can organize the mapping as you feel more confortable for your application; in some case a class is enough, in other cases would
			// be better the separation per module/concern, you are not closed in "mapping per class" box.

			var orm = new ObjectRelationalMapper();

			// With the follow line I'm adding the pattern for the POID strategy ("native" instead the default "High-Low")
			orm.Patterns.PoidStrategies.Add(new NativePoidPattern());

			// composition of patterns-appliers-packs and patterns-appliers for Lorenzo's domain
			// Note: for bidirectional-one-to-many association Lorenzo is not interested in the default cascade behavior,
			// implemented in the BidirectionalOneToManyRelationPack, because he is using a sort of soft-delete in his base class VersionModelBase.
			// He can implements a custom pack of patterns-appliers to manage the situation when classes does not inherits from VersionModelBase by the way
			// he don't want the cascade atall, so the BidirectionalOneToManyInverseApplier will be enough
			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolColumnsNamingPack(orm))
					.Merge(new TablePerClassPack())
					.Merge(new PluralizedTablesPack(orm, new EnglishInflector()))
					.Merge(new ListIndexAsPropertyPosColumnNameApplier())
					.Merge(new BidirectionalOneToManyInverseApplier(orm))
					.Merge(new EnumAsStringPack())
					.Merge(new DatePropertyByNameApplier())
					.Merge(new MsSQL2008DateTimeApplier());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			// Setting the version property using the base class
			orm.VersionProperty<VersionModelBase>(v => v.Version);

			// Note: I'm declaring the strategy only for the base entity
			orm.TablePerClassHierarchy<Cost>();
			orm.TablePerClass<EditionQuotation>();
			orm.TablePerClass<ProductQuotation>();
			orm.TablePerClass<Quotation>();

			AppliesGeneralConventions(mapper);

			// EditionQuotation.PrintCost don't use lazy-loading
			mapper.Customize<EditionQuotation>(map => map.ManyToOne(eq => eq.PrintCost, m2o => m2o.Lazy(LazyRelation.NoLazy)));

			// Customizes some others DDL's stuff outside conventions
			CustomizeColumns(mapper);

			// Note : I have to create mappings for the whole domain
			HbmMapping mapping =
				mapper.CompileMappingFor(
					typeof (GenericModelBase<>).Assembly.GetTypes().Where(t => t.Namespace == typeof (GenericModelBase<>).Namespace));
			Console.Write(mapping.AsString());
		}

		private void AppliesGeneralConventions(Mapper mapper)
		{
			const string softDeleteWhereClause = "DeletedOn is NULL";

			// because VersionModelBase is not an entity I can use the it to customize the mapping of all inherited classes
			mapper.Class<VersionModelBase>(map => map.Where(softDeleteWhereClause));

			// When the collection elements inherits from VersionModelBase then should apply the where-clause for "soft delete"
			mapper.AddCollectionPattern(mi => mi.GetPropertyOrFieldType().IsGenericCollection() &&
			                                  typeof (VersionModelBase).IsAssignableFrom(
			                                  	mi.GetPropertyOrFieldType().DetermineCollectionElementType())
			                            , map => map.Where(softDeleteWhereClause));

			// The default length for string is 100
			// Note : because this convetion has no specific restriction other than typeof(string) should be added at first (the order, in this case, is important)
			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string), map => map.Length(100));

			// When the property name is Description and is of type string then apply length 500
			mapper.AddPropertyPattern(mi => mi.Name == "Description" && mi.GetPropertyOrFieldType() == typeof (string),
			                          map => map.Length(500));

			// When the property is of type decimal and it refers to a price then applies Currency
			mapper.AddPropertyPattern(mi => mi.Name.EndsWith("Cost") && mi.GetPropertyOrFieldType() == typeof (decimal),
			                          map => map.Type(NHibernateUtil.Currency));
		}

		private void CustomizeColumns(Mapper mapper)
		{
			// in EditionQuotation all references to a class inherited from Cost are not nullables (this is only an example because you can do it one by one)
			mapper.AddManyToOnePattern(
				mi => mi.DeclaringType == typeof (EditionQuotation) && typeof (Cost).IsAssignableFrom(mi.GetPropertyOrFieldType()),
				map => map.NotNullable(true));

			// in Quotation all string properties are not nullables (this is only an example because you can do it one by one)
			mapper.AddPropertyPattern(
				mi => mi.DeclaringType == typeof (Quotation) && mi.GetPropertyOrFieldType() == typeof (string),
				map => map.NotNullable(true));

			// Quotation columns size customization (if you use a Validator framework you can implements a pattern-applier to customize columns size and "nullable" stuff)
			// Note: It is needed only for properties without a convention (the property Description has its convetion)
			mapper.Customize<Quotation>(map =>
			                            	{
			                            		map.Property(q => q.Code, pm => pm.Length(10));
			                            		map.Property(q => q.Reference, pm => pm.Length(50));
			                            		map.Property(q => q.PaymentMethod, pm => pm.Length(50));
			                            		map.Property(q => q.TransportMethod, pm => pm.Length(50));
			                            	});

			// Column customization for class ProductQuotation
			// Note: It is needed only for properties outside conventions
			mapper.Customize<ProductQuotation>(map => map.Property(pq => pq.Group, pm =>
			                                                                       	{
			                                                                       		pm.Length(50);
			                                                                       		pm.Column("GroupName");
			                                                                       	}));
		}
	}
}