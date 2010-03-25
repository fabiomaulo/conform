using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace ConfOrm.NH
{
	public class AnyMapper : IAnyMapper
	{
		private const string DefaultIdColumnNameWhenNoProperty = "ReferencedId";
		private const string DefaultMetaColumnNameWhenNoProperty = "ReferencedClass";
		private readonly MemberInfo member;
		private readonly Type foreignIdType;
		private readonly HbmAny any;
		private readonly IEntityPropertyMapper entityPropertyMapper;
		private readonly ColumnMapper idColumnMapper;
		private readonly ColumnMapper classColumnMapper;

		public AnyMapper(MemberInfo member, Type foreignIdType, HbmAny any)
		{
			this.member = member;
			this.foreignIdType = foreignIdType;
			this.any = any;
			if (member == null)
			{
				this.any.access = "none";
			}
			if (member == null)
			{
				entityPropertyMapper = new NoMemberPropertyMapper();
			}
			else
			{
				entityPropertyMapper = new EntityPropertyMapper(member.DeclaringType, member.Name, x => any.access = x);
			}
			if (foreignIdType == null)
			{
				throw new ArgumentNullException("foreignIdType");
			}
			if (any == null)
			{
				throw new ArgumentNullException("any");
			}

			this.any.idtype = this.foreignIdType.GetNhTypeName();
			var idHbmColumn = new HbmColumn();
			var idColumnName = member == null ? DefaultIdColumnNameWhenNoProperty : member.Name + "Id";
			idColumnMapper = new ColumnMapper(idHbmColumn, idColumnName);
			var classHbmColumn = new HbmColumn();
			var classColumnName = member == null ? DefaultMetaColumnNameWhenNoProperty : member.Name + "Class";
			classColumnMapper = new ColumnMapper(classHbmColumn, classColumnName);
			any.column = new[] {idHbmColumn, classHbmColumn};
		}

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

		#region Implementation of IAnyMapper

		public void MetaType(IType metaType)
		{
			if(metaType != null)
			{
				any.metatype = metaType.Name;
			}
		}

		public void MetaType<TMetaType>()
		{
			MetaType(typeof (TMetaType));
		}

		public void MetaType(Type metaType)
		{
			if (metaType != null)
			{
				any.metatype = metaType.GetNhTypeName();
			}
		}

		public void IdType(IType idType)
		{
			if (idType != null)
			{
				any.idtype = idType.Name;
			}
		}

		public void IdType<TIdType>()
		{
			IdType(typeof (TIdType));
		}

		public void IdType(Type idType)
		{
			if (idType != null)
			{
				any.idtype = idType.GetNhTypeName();
			}
		}

		public void Columns(Action<IColumnMapper> idColumnMapping, Action<IColumnMapper> classColumnMapping)
		{
			throw new NotImplementedException();
		}

		public void MetaValue(object value, Type entityType)
		{
			throw new NotImplementedException();
		}

		public void Cascade(Cascade cascadeStyle)
		{
			any.cascade = (cascadeStyle & ~ConfOrm.Cascade.DeleteOrphans).ToCascadeString();
		}

		public void Index(string indexName)
		{
			any.index = indexName;
		}

		public void Lazy(bool isLazy)
		{
			any.lazy = isLazy;
		}

		#endregion
	}
}