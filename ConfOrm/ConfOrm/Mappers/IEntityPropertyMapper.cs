using NHibernate.Properties;

namespace ConfOrm.Mappers
{
	public enum Accessor
	{
		Property,
		Field,
		ReadOnly,
		None
	}
	public interface IEntityPropertyMapper
	{
		void Access(Accessor accessor);
		void Access<TAccessor>() where TAccessor : IPropertyAccessor;
	}
}