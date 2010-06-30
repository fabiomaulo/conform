using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ParentMapper: IParentMapper
	{
		private readonly AccessorPropertyMapper accessorPropertyMapper;

		public ParentMapper(HbmParent parent, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			parent.name = member.Name;
			accessorPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => parent.access = x);
		}

		public void Access(Accessor accessor)
		{
			accessorPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			accessorPropertyMapper.Access(accessorType);
		}
	}
}