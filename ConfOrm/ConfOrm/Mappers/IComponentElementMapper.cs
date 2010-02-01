using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentElementMapper
	{
		void Property(MemberInfo property);

		void Component(MemberInfo property, Action<IComponentElementMapper> mapping);

		void ManyToOne(MemberInfo property);
	}
}