using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IClassMapper : IPropertyContainerMapper
	{
		void Id(Action<IIdMapper> idMapper);
		void Id(MemberInfo idProperty, Action<IIdMapper> idMapper);
		void Discriminator();
	}
}