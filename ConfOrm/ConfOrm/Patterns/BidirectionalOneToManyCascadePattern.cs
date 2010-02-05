namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyCascadePattern : BidirectionalOneToManyPattern, IPatternApplier<Relation, Cascade>
	{
		#region Implementation of IPatternApplier<Relation,Cascade>

		public Cascade Apply(Relation subject)
		{
			return Cascade.All | Cascade.DeleteOrphans;
		}

		#endregion
	}
}