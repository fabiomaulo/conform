using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentElementMapper
	{
		void Parent(MemberInfo parent);

		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentElementMapper> mapping);

		void ManyToOne(MemberInfo property);
	}
}