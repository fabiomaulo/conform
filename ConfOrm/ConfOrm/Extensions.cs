namespace ConfOrm
{
	public static class Extensions
	{
		internal const Cascade EachButDeleteOrphans = Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove
																							 | Cascade.Detach | Cascade.ReAttach | Cascade.All;

		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}
	}
}