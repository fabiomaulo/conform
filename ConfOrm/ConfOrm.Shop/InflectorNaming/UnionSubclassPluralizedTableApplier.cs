using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class UnionSubclassPluralizedTableApplier : IPatternApplier<Type, IUnionSubclassMapper>
	{
		private readonly IInflector inflector;

		public UnionSubclassPluralizedTableApplier(IInflector inflector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		#region IPatternApplier<Type,IUnionSubclassMapper> Members

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IUnionSubclassMapper applyTo)
		{
			applyTo.Table(inflector.Pluralize(subject.Name));
		}

		#endregion
	}
}