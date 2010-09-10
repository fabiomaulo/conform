using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class ClassPluralizedTableApplier: IPatternApplier<Type, IClassMapper>
	{
		private readonly IInflector inflector;

		public ClassPluralizedTableApplier(IInflector inflector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IClassMapper applyTo)
		{
			applyTo.Table(inflector.Pluralize(subject.Name));
		}
	}
}