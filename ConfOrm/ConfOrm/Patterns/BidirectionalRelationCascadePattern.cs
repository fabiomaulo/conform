namespace ConfOrm.Patterns
{
	public class BidirectionalRelationCascadePattern: BidirectionalRelationPattern, IPatternApplier<Relation, Cascade>
	{
		#region Implementation of IPatternApplier<Relation,Cascade>

		public Cascade Apply(Relation subject)
		{
			return Cascade.All;
		}

		#endregion
	}
}