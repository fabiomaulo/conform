namespace ConfOrm
{
	public static class Extensions
	{
		private const CascadeOn AnyButOrphans = CascadeOn.Persist | CascadeOn.Refresh | CascadeOn.Merge | CascadeOn.Remove | CascadeOn.Detach | CascadeOn.ReAttach;

		public static bool Has(this CascadeOn source, CascadeOn value)
		{
			return (source & value) == value;
		}

		public static CascadeOn Include(this CascadeOn source, CascadeOn value)
		{
			return Cleanup(source | value);
		}

		private static CascadeOn Cleanup(CascadeOn cascade)
		{
			bool hasAll = cascade.Has(CascadeOn.All) || cascade.Has(AnyButOrphans);
			if (hasAll && cascade.Has(CascadeOn.DeleteOrphans))
			{
				return CascadeOn.All | CascadeOn.DeleteOrphans;
			}
			if (hasAll)
			{
				return CascadeOn.All;
			}
			return cascade;
		}

		public static CascadeOn Exclude(this CascadeOn source, CascadeOn value)
		{
			if(source.Has(CascadeOn.All) && !value.Has(CascadeOn.All))
			{
				return Cleanup(((source & ~CascadeOn.All) | AnyButOrphans) & ~value);
			}
			return Cleanup(source & ~value);
		}
	}
}