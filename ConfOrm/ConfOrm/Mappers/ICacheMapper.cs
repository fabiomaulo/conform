namespace ConfOrm.Mappers
{
	public interface ICacheMapper
	{
		void Usage(CacheUsage cacheUsage);
		void Region(string regionName);
		void Include(CacheInclude cacheUsage);
	}
}