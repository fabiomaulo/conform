namespace ConfOrm.Mappers
{
	public interface IMapKeyManyToManyMapper : IColumnsMapper
	{
		void ForeignKey(string foreignKeyName);
	}
}