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
		private readonly IIdMapper idMapper;
		private IVersionMapper versionMapper;
		private INaturalIdMapper naturalIdMapper;
		private ICacheMapper cacheMapper;

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
			
			var hbmId = new HbmId();
			classMapping.Item = hbmId;
			idMapper = new IdMapper(idProperty, hbmId);

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

		public void Id(Action<IIdMapper> mapper)
		{
			mapper(idMapper);
		}

		public void Id(MemberInfo idProperty, Action<IIdMapper> mapper)
		{
			var id = (HbmId)classMapping.Item;
			mapper(new IdMapper(idProperty, id));
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
			if (value != null)
			{
				classMapping.discriminatorvalue = value.ToString();
				Discriminator();
				var valueType = value.GetType();
				if(valueType != typeof(string))
				{
					classMapping.discriminator.type = valueType.GetNhTypeName();
				}
			}
			else
			{
				classMapping.discriminatorvalue = "null";
			}
		}

		public void Table(string tableName)
		{
			classMapping.table = tableName;
		}

		public void Catalog(string catalogName)
		{
			classMapping.catalog = catalogName;
		}

		public void Schema(string schemaName)
		{
			classMapping.schema = schemaName;
		}

		public void Mutable(bool isMutable)
		{
			classMapping.mutable = isMutable;
		}

		public void Version(MemberInfo versionProperty, Action<IVersionMapper> versionMapping)
		{
			if(versionMapper == null)
			{
				var hbmVersion = new HbmVersion();
				classMapping.Item1 = hbmVersion;
				versionMapper = new VersionMapper(versionProperty, hbmVersion);
			}
			versionMapping(versionMapper);
		}

		public void NaturalId(Action<INaturalIdMapper> naturalIdMapping)
		{
			if(naturalIdMapper == null)
			{
				var hbmNaturalId = new HbmNaturalId();
				classMapping.naturalid = hbmNaturalId;
				naturalIdMapper = new NaturalIdMapper(Container, hbmNaturalId, MapDoc);
			}
			naturalIdMapping(naturalIdMapper);
		}

		public void Cache(Action<ICacheMapper> cacheMapping)
		{
			if (cacheMapper == null)
			{
				var hbmCache = new HbmCache();
				classMapping.cache = hbmCache;
				cacheMapper = new CacheMapper(hbmCache);
			}
			cacheMapping(cacheMapper);
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