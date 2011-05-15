using System.Collections.Generic;
using System.Linq;

namespace ConfOrm.NH
{
	public static class CascadeConverter
	{
		public static string ToCascadeString(this CascadeOn source)
		{
			return source == CascadeOn.None ? null : string.Join(",", source.CascadeDefinitions().ToArray());
		}

		private static IEnumerable<string> CascadeDefinitions(this CascadeOn source)
		{
			if (source.Has(CascadeOn.All))
			{
				yield return "all";				
			}
			if (source.Has(CascadeOn.Persist))
			{
				yield return "save-update, persist";
			}
			if (source.Has(CascadeOn.Refresh))
			{
				yield return "refresh";
			}
			if (source.Has(CascadeOn.Merge))
			{
				yield return "merge";
			}
			if (source.Has(CascadeOn.Remove))
			{
				yield return "delete";
			}
			if (source.Has(CascadeOn.Detach))
			{
				yield return "evict";
			}
			if (source.Has(CascadeOn.ReAttach))
			{
				yield return "lock";
			}
			if (source.Has(CascadeOn.DeleteOrphans))
			{
				yield return "delete-orphan";
			}
		}
	}
}