using System.Collections.Generic;
using System.Linq;

namespace ConfOrm.NH
{
	public static class CascadeConverter
	{
		internal const Cascade EachButDeleteOrphans = Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove
		                                               | Cascade.Detach | Cascade.ReAttach | Cascade.All;

		public static string ToCascadeString(this Cascade source)
		{
			return string.Join(",", source.CascadeDefinitions().ToArray());
		}

		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}

		private static IEnumerable<string> CascadeDefinitions(this Cascade source)
		{
			if (source.Has(Cascade.All))
			{
				yield return "all";				
			}
			if (source.Has(Cascade.Persist))
			{
				yield return "save-update, persist";
			}
			if (source.Has(Cascade.Refresh))
			{
				yield return "refresh";
			}
			if (source.Has(Cascade.Merge))
			{
				yield return "merge";
			}
			if (source.Has(Cascade.Remove))
			{
				yield return "delete";
			}
			if (source.Has(Cascade.Detach))
			{
				yield return "evict";
			}
			if (source.Has(Cascade.ReAttach))
			{
				yield return "lock";
			}
			if (source.Has(Cascade.DeleteOrphans))
			{
				yield return "delete-orphan";
			}
		}
	}
}