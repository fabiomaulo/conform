using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class OneToOneMapper: IOneToOneMapper
	{
		private readonly MemberInfo member;
		private readonly HbmOneToOne oneToOne;
		private readonly IEntityPropertyMapper entityPropertyMapper;

		public OneToOneMapper(MemberInfo member, HbmOneToOne oneToOne)
		{
			this.member = member;
			this.oneToOne = oneToOne;
			if (member == null)
			{
				this.oneToOne.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new EntityPropertyMapper(member.DeclaringType, member.Name, x => oneToOne.access = x);
			}
		}

		#region Implementation of IOneToOneMapper

		public void Cascade(Cascade cascadeStyle)
		{
			oneToOne.cascade = (cascadeStyle & ~ConfOrm.Cascade.DeleteOrphans).ToCascadeString();
		}

		#endregion

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		#endregion

		public void Lazy(LazyRelation lazyRelation)
		{
			oneToOne.lazy = lazyRelation.ToHbm();
			oneToOne.lazySpecified = oneToOne.lazy != HbmLaziness.Proxy;
		}
	}
}