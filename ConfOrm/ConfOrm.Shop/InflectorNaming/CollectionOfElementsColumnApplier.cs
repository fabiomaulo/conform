using ConfOrm.Shop.Inflectors;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.Shop.InflectorNaming
{
	public class CollectionOfElementsColumnApplier : Appliers.CollectionOfElementsColumnApplier
	{
		private readonly IInflector inflector;

		public CollectionOfElementsColumnApplier(IDomainInspector domainInspector, IInflector inflector)
			: base(domainInspector)
		{
			this.inflector = inflector;
		}

		protected override string GetColumnName(PropertyPath subject)
		{
			return inflector.Singularize(subject.ToColumnName());
		}
	}
}