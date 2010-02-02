using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public static class CascadeConverter
	{
		public static string ToCascadeString(this Cascade source)
		{
			if (source.Has(Cascade.All))
			{
				return "all, delete-orphans";
			}
			return string.Join(",", source.CascadeDefinitions().ToArray());
		}

		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}

		private static IEnumerable<string> CascadeDefinitions(this Cascade source)
		{
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
				yield return "delete-orphans";
			}
		}
	}
}