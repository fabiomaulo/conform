using System;

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
		void Access(Type accessorType);
	}
}