using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class JoinedSubclassPluralizedTableApplier : IPatternApplier<Type, IJoinedSubclassMapper>
	{
		private readonly IInflector inflector;

		public JoinedSubclassPluralizedTableApplier(IInflector inflector)
		{
			if (inflector == null)
			{
				throw new ArgumentNullException("inflector");
			}
			this.inflector = inflector;
		}

		#region IPatternApplier<Type,IJoinedSubclassMapper> Members

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IJoinedSubclassMapper applyTo)
		{
			applyTo.Table(inflector.Pluralize(subject.Name));
		}

		#endregion
	}
}