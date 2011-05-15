using System;
using NHibernate.Mapping.ByCode;

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

		public static Cascade ToCascade(this CascadeOn source)
		{
			// so far can be done with another trick but I want prevent problems if the values/names will change in NHibernate
			var result = Cascade.None;
			result = IncludeIfNeeded(source, CascadeOn.Persist, result);
			result = IncludeIfNeeded(source, CascadeOn.Refresh, result);
			result = IncludeIfNeeded(source, CascadeOn.Merge, result);
			result = IncludeIfNeeded(source, CascadeOn.Remove, result);
			result = IncludeIfNeeded(source, CascadeOn.Detach, result);
			result = IncludeIfNeeded(source, CascadeOn.ReAttach, result);
			result = IncludeIfNeeded(source, CascadeOn.DeleteOrphans, result);
			result = IncludeIfNeeded(source, CascadeOn.All, result);
			return result;
		}

		private static Cascade IncludeIfNeeded(CascadeOn source, CascadeOn valueToCheck, Cascade destination)
		{
			return source.Has(valueToCheck) ? destination.Include(ConvertSingleValue(valueToCheck)) : destination;
		}

		private static Cascade ConvertSingleValue(CascadeOn source)
		{
			switch (source)
			{
				case CascadeOn.None:
					return Cascade.None;
				case CascadeOn.Persist:
					return Cascade.Persist;
				case CascadeOn.Refresh:
					return Cascade.Refresh;
				case CascadeOn.Merge:
					return Cascade.Merge;
				case CascadeOn.Remove:
					return Cascade.Remove;
				case CascadeOn.Detach:
					return Cascade.Detach;
				case CascadeOn.ReAttach:
					return Cascade.ReAttach;
				case CascadeOn.DeleteOrphans:
					return Cascade.DeleteOrphans;
				case CascadeOn.All:
					return Cascade.All;
				default:
					throw new ArgumentOutOfRangeException("source");
			}
		}
	}
}