using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ClassMapper: AbstractPropertyContainerMapper, IClassMapper
	{
		private readonly HbmClass classMapping;

		public ClassMapper(Type rootClass, HbmMapping mapDoc, MemberInfo idProperty)
			: base(rootClass, mapDoc)
		{
			classMapping = new HbmClass();
			var toAdd = new[] { classMapping };
			classMapping.name = rootClass.GetShortClassName(mapDoc);
			if(rootClass.IsAbstract)
			{
				classMapping.@abstract = true;
				classMapping.abstractSpecified = true;
			}
			if (idProperty != null)
			{
				var idType = GetMemberType(idProperty);
				classMapping.Item = new HbmId {name = idProperty.Name, type1 = idType.GetNhTypeName()};
			}
			else
			{
				classMapping.Item = new HbmId();				
			}
			mapDoc.Items = mapDoc.Items == null ? toAdd : mapDoc.Items.Concat(toAdd).ToArray();
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			classMapping.Items = classMapping.Items == null ? toAdd : classMapping.Items.Concat(toAdd).ToArray();
		}

		#endregion

		#region Implementation of IClassMapper

		public void Id(Action<IIdMapper> idMapper)
		{
			idMapper(new IdMapper((HbmId)classMapping.Item));
		}

		public void Id(MemberInfo idProperty, Action<IIdMapper> idMapper)
		{
			var idType = GetMemberType(idProperty);
			var id = (HbmId)classMapping.Item;
			id.name = idProperty.Name;
			id.type1 = idType.GetNhTypeName();
			idMapper(new IdMapper(id));
		}

		public void Discriminator()
		{
			if (classMapping.discriminator == null)
			{
				classMapping.discriminator = new HbmDiscriminator();
			}
		}

		public void DiscriminatorValue(object value)
		{
			classMapping.discriminatorvalue = value != null ? value.ToString() : "null";
		}

		#endregion

		#region Implementation of IEntityAttributesMapper

		public void EntityName(string value)
		{
			classMapping.entityname = value;
		}

		public void Proxy(Type proxy)
		{
			if (!Container.IsAssignableFrom(proxy) && !proxy.IsAssignableFrom(Container))
			{
				throw new MappingException("Not compatible proxy for " + Container);
			}
			classMapping.proxy = proxy.GetShortClassName(MapDoc);
		}

		public void Lazy(bool value)
		{
			classMapping.lazy = value;
			classMapping.lazySpecified = !value;
		}

		public void DynamicUpdate(bool value)
		{
			classMapping.dynamicupdate = value;
		}

		public void DynamicInsert(bool value)
		{
			classMapping.dynamicinsert = value;
		}

		public void BatchSize(int value)
		{
			if (value > 0)
			{
				classMapping.batchsize = value;
				classMapping.batchsizeSpecified = true;
			}
			else
			{
				classMapping.batchsize = 0;
				classMapping.batchsizeSpecified = false;
			}
		}

		public void SelectBeforeUpdate(bool value)
		{
			classMapping.selectbeforeupdate = value;
		}

		#endregion

		#region Implementation of IEntitySqlsMapper

		public void Loader(string namedQueryReference)
		{
			if(classMapping.SqlLoader == null)
			{
				classMapping.loader = new HbmLoader();
			}
			classMapping.loader.queryref = namedQueryReference;
		}

		public void SqlInsert(string sql)
		{
			if (classMapping.SqlInsert == null)
			{
				classMapping.sqlinsert = new HbmCustomSQL();
			}
			classMapping.sqlinsert.Text = new[] { sql };
		}

		public void SqlUpdate(string sql)
		{
			if (classMapping.SqlUpdate == null)
			{
				classMapping.sqlupdate = new HbmCustomSQL();
			}
			classMapping.sqlupdate.Text = new[] { sql };
		}

		public void SqlDelete(string sql)
		{
			if (classMapping.SqlDelete == null)
			{
				classMapping.sqldelete = new HbmCustomSQL();
			}
			classMapping.sqldelete.Text = new[] { sql };
		}

		#endregion
	}
}