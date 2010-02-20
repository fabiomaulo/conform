using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class PropertyMapper: IPropertyMapper
	{
		private class NoMemberPropertyMapper : IEntityPropertyMapper
		{
			public void Access(Accessor accessor) {}

			public void Access(Type accessorType) {}
		}
		private readonly MemberInfo member;
		private readonly HbmProperty propertyMapping;
		private readonly IEntityPropertyMapper entityPropertyMapper;

		public PropertyMapper(MemberInfo member, HbmProperty propertyMapping)
		{
			if (propertyMapping == null)
			{
				throw new ArgumentNullException("propertyMapping");
			}
			this.member = member;
			this.propertyMapping = propertyMapping;
			if(member == null)
			{
				this.propertyMapping.access = "none";				
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new EntityPropertyMapper(member.DeclaringType, member.Name, x => propertyMapping.access = x);
			}
		}

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			entityPropertyMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			entityPropertyMapper.Access(accessorType);
		}

		#endregion

		#region Implementation of IPropertyMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				propertyMapping.type1 = persistentType.Name;
				propertyMapping.type = null;
			}
		}

		public void Type<TPersistentType>() where TPersistentType : IUserType
		{
			Type(typeof (TPersistentType), null);
		}

		public void Type<TPersistentType>(object parameters) where TPersistentType : IUserType
		{
			Type(typeof(TPersistentType), parameters);
		}

		public void Type(Type persistentType, object parameters)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof(IUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType");
			}
			if (parameters != null)
			{
				propertyMapping.type1 = null;
				var hbmType = new HbmType
				{
					name = persistentType.AssemblyQualifiedName,
					param = (from pi in parameters.GetType().GetProperties()
									 let pname = pi.Name
									 let pvalue = pi.GetValue(parameters, null)
									 select
										new HbmParam { name = pname, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } })
						.ToArray()
				};
				propertyMapping.type = hbmType;
			}
			else
			{
				propertyMapping.type1 = persistentType.AssemblyQualifiedName;
				propertyMapping.type = null;
			}
		}

		#endregion
	}
}