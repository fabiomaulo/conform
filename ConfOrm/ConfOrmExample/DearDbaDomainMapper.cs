using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.DearDbaNaming;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.Packs;

namespace ConfOrmExample
{
	public class DearDbaDomainMapper : IDomainMapper
	{
		private readonly ObjectRelationalMapper orm;
		private readonly Mapper mapper;
		public DearDbaDomainMapper()
		{
			orm = new ObjectRelationalMapper();

			// Remove one of not required patterns
			orm.Patterns.ManyToOneRelations.Remove(
				orm.Patterns.ManyToOneRelations.Single(p => p.GetType() == typeof(OneToOneUnidirectionalToManyToOnePattern)));

			orm.Patterns.PoidStrategies.Add(new NativePoidPattern());
			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new DearDbaTablesAndColumnsNamingPack(orm, new EnglishInflector()))
					.Merge(new ListIndexAsPropertyPosColumnNameApplier())
					.Merge(new TablePerClassPack())
					.Merge(new DatePropertyByNameApplier())
					.Merge(new MsSQL2008DateTimeApplier());

			patternsAppliers.Merge(new DatePropertyByNameApplier()).Merge(new MsSQL2008DateTimeApplier());
			mapper = new Mapper(orm, patternsAppliers);
		}

		#region Implementation of IDomainMapper

		public ObjectRelationalMapper DomainDefinition
		{
			get { return orm; }
		}

		public Mapper Mapper
		{
			get { return mapper; }
		}

		#endregion
	}
}