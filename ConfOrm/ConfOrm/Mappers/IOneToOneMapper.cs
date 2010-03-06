namespace ConfOrm.Mappers
{
	public interface IOneToOneMapper: IEntityPropertyMapper
	{
		void Cascade(Cascade cascadeStyle);
	}
}