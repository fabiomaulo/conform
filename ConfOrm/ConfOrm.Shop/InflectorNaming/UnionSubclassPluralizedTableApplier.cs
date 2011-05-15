using System;
using NHibernate.Mapping.ByCode;
using ConfOrm.Shop.Inflectors;

namespace ConfOrm.Shop.InflectorNaming
{
	public class UnionSubclassPluralizedTableApplier : IPatternApplier<Type, IUnionSubclassAttributesMapper>
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

		public IInflector Inflector
		{
			get { return inflector; }
		}

		#region IPatternApplier<Type,IUnionSubclassAttributesMapper> Members

		public bool Match(Type subject)
		{
			// this pattern is called only for root-entities
			return subject != null;
		}

		public void Apply(Type subject, IUnionSubclassAttributesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		public virtual string GetTableName(Type subject)
		{
			return inflector.Pluralize(subject.Name);
		}

		#endregion
	}
}