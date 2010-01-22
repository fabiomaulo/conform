namespace ConfOrm.Mappers
{
	public interface IClassMapper : IPropertyContainerMapper
	{
		void Discriminator();
	}
}