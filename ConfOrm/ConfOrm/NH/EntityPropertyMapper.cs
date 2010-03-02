using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class EntityPropertyMapper : AccessorPropertyMapper, IEntityPropertyMapper
	{
		public EntityPropertyMapper(Type declaringType, string propertyName, Action<string> accesorValueSetter) : base(declaringType, propertyName, accesorValueSetter)
		{}
	}
}