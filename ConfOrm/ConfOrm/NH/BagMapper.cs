using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class BagMapper : IBagPropertiesMapper
	{
		private readonly KeyMapper keyMapper;
		private readonly HbmBag mapping;
		private readonly IEntityPropertyMapper entityPropertyMapper;

		public BagMapper(Type ownerType, Type elementType, HbmBag mapping)
		{
			if (ownerType == null)
			{
				throw new ArgumentNullException("ownerType");
			}
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			OwnerType = ownerType;
			ElementType = elementType;
			this.mapping = mapping;
			if (mapping.Key == null)
			{
				mapping.key = new HbmKey();
			}
			keyMapper = new KeyMapper(ownerType, mapping.Key);
			entityPropertyMapper = new EntityPropertyMapper(ownerType, mapping.Name, x => mapping.access = x);
		}

		public Type OwnerType { get; private set; }
		public Type ElementType { get; private set; }

		#region Implementation of ICollectionPropertiesMapper

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

		public void Sort() {}
		public void Sort<TComparer>() {}

		public void Cascade(Cascade cascadeStyle)
		{
			mapping.cascade = cascadeStyle.ToCascadeString();
		}

		public void Type<TCollection>() where TCollection : IUserCollectionType
		{
			mapping.collectiontype = typeof (TCollection).AssemblyQualifiedName;
		}

		public void Type(Type collectionType)
		{
			if (collectionType == null)
			{
				throw new ArgumentNullException("collectionType");
			}
			if (!typeof (IUserCollectionType).IsAssignableFrom(collectionType))
			{
				throw new ArgumentOutOfRangeException("collectionType",
				                                      string.Format(
				                                                   	"The collection type should be an implementation of IUserCollectionType.({0})",
				                                                   	collectionType));
			}
			mapping.collectiontype = collectionType.AssemblyQualifiedName;
		}

		#endregion

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
	}
}