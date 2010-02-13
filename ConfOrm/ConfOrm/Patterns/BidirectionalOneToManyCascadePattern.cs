using System;
using System.Collections;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyCascadePattern : BidirectionalOneToManyPattern, IPatternValueGetter<Relation, Cascade>
	{
		public override bool Match(Relation subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			Type one = subject.From;
			Type many = subject.To;
			if (one.Equals(many))
			{
				// Circular references
				var relationOn = subject as RelationOn;
				if (relationOn != null && !typeof(IEnumerable).IsAssignableFrom(relationOn.On.GetPropertyOrFieldType()))
				{
					// the many-side (the collection) match, but the one-side (the 'parent') is the AggregateRoot and shouldn't match
					return false;
				}
			}

			return base.Match(subject);
		}
		#region Implementation of IPatternApplier<Relation,Cascade>

		public Cascade Get(Relation element)
		{
			return Cascade.All | Cascade.DeleteOrphans;
		}

		#endregion
	}
}