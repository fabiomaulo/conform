namespace ConfOrm
{
	public static class Extensions
	{
		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}

		public static Cascade Include(this Cascade source, Cascade value)
		{
			return source | value;
		}

		public static Cascade Exclude(this Cascade source, Cascade value)
		{
			return source & ~value;
		}
	}
}