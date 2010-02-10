using System;

namespace ConfOrm.Mappers
{
	public enum Accessor
	{
		Property,
		Field,
		NoSetter,
		ReadOnly,
		None
	}

	public interface IEntityPropertyMapper
	{
		void Access(Accessor accessor);
		void Access(Type accessorType);
	}
}