using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;

namespace ConfOrmExample
{
	public class CoolDomainMapper: IDomainMapper
	{
		private readonly ObjectRelationalMapper orm;
		private readonly Mapper mapper;
		public CoolDomainMapper()
		{
			orm = new ObjectRelationalMapper();

			// Remove one of not required patterns
			orm.Patterns.ManyToOneRelations.Remove(
				orm.Patterns.ManyToOneRelations.Single(p => p.GetType() == typeof (OneToOneUnidirectionalToManyToOnePattern)));

			orm.Patterns.PoidStrategies.Add(new NativePoidPattern());
			var patternsAppliers = new CoolPatternsAppliersHolder(orm);
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