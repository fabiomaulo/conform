using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class MapMapper : IMapPropertiesMapper
	{
		private readonly KeyMapper keyMapper;
		private readonly HbmMap mapping;
		private readonly HbmMapping mapDoc;

		public MapMapper(Type ownerType, Type keyType, Type valueType, HbmMap mapping, HbmMapping mapDoc)
		{
			if (ownerType == null)
			{
				throw new ArgumentNullException("ownerType");
			}
			if (keyType == null)
			{
				throw new ArgumentNullException("keyType");
			}
			if (valueType == null)
			{
				throw new ArgumentNullException("valueType");
			}
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			OwnerType = ownerType;
			KeyType = keyType;
			ValueType = valueType;
			this.mapping = mapping;
			this.mapDoc = mapDoc;
			if (mapping.Key == null)
			{
				mapping.key = new HbmKey();
			}
			keyMapper = new KeyMapper(ownerType, mapping.Key);

			if (KeyType.IsValueType || KeyType == typeof(string))
			{
				mapping.Item = new HbmMapKey { type = KeyType.GetNhTypeName() };
			}
			else
			{
				mapping.Item = new HbmMapKeyManyToMany { @class = KeyType.GetShortClassName(mapDoc) };
			}
		}

		public Type OwnerType { get; private set; }
		public Type KeyType { get; private set; }
		public Type ValueType { get; private set; }

		#region Implementation of IMapPropertiesMapper

		public void Inverse(bool value)
		{
			mapping.inverse = value;
		}

		public void Mutable(bool value)
		{
			mapping.mutable = value;
		}

		public void Where(string sqlWhereClause)
		{
			mapping.where = sqlWhereClause;
		}

		public void BatchSize(int value)
		{
			if (value > 0)
			{
				mapping.batchsize = value;
				mapping.batchsizeSpecified = true;
			}
			else
			{
				mapping.batchsize = 0;
				mapping.batchsizeSpecified = false;
			}
		}

		public void Lazy(CollectionLazy collectionLazy)
		{
			mapping.lazySpecified = true;
			switch (collectionLazy)
			{
				case CollectionLazy.Lazy:
					mapping.lazy = HbmCollectionLazy.True;
					break;
				case CollectionLazy.NoLazy:
					mapping.lazy = HbmCollectionLazy.False;
					break;
				case CollectionLazy.Extra:
					mapping.lazy = HbmCollectionLazy.Extra;
					break;
			}
		}

		public void Key(Action<IKeyMapper> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public void OrderBy(MemberInfo property)
		{
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			mapping.orderby = property.Name;
		}

		public void Sort()
		{
			mapping.sort = "natural";
		}

		public void Cascade(Cascade cascadeStyle)
		{
			mapping.cascade = cascadeStyle.ToCascadeString();
		}

		public void MapKeyManyToMany(Action<IMapKeyManyToManyMapper> mapKeyMapping)
		{
			var mkManyToMany = mapping.Item as HbmMapKeyManyToMany;
			if (mkManyToMany == null)
			{
				mapping.Item = new HbmMapKeyManyToMany {@class = KeyType.GetShortClassName(mapDoc)};
			}
			mapKeyMapping(null);
		}

		#endregion
	}
}