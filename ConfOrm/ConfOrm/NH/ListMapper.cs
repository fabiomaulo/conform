using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ListMapper : ICollectionPropertiesMapper
	{
		private readonly KeyMapper keyMapper;
		private readonly HbmList mapping;

		public ListMapper(Type ownerType, Type elementType, HbmList mapping)
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
			mapping.Item = new HbmListIndex();
		}

		public Type OwnerType { get; private set; }
		public Type ElementType { get; private set; }

		#region Implementation of ICollectionPropertiesMapper

		public void Key(Action<IKeyMapper> keyMapping)
		{
			keyMapping(keyMapper);
		}

		public bool Inverse
		{
			get { return mapping.Inverse; }
			set { mapping.inverse = value; }
		}

		public bool Mutable
		{
			get { return mapping.Mutable; }
			set { mapping.mutable = value; }
		}

		public void OrderBy(MemberInfo property)
		{
			// TODO: read the mapping of the element to know the column of the property (second-pass)
			mapping.orderby = property.Name;
		}

		public string Where
		{
			get { return mapping.Where; }
			set { mapping.where = value; }
		}

		public int BatchSize
		{
			get { return mapping.BatchSize.GetValueOrDefault(-1); }
			set
			{
				mapping.batchsizeSpecified = true;
				mapping.batchsize = value;
			}
		}

		public CollectionLazy Lazy
		{
			get
			{
				if (!mapping.Lazy.HasValue)
				{
					return CollectionLazy.Lazy;
				}
				switch (mapping.Lazy.Value)
				{
					case HbmCollectionLazy.True:
						return CollectionLazy.Lazy;
					case HbmCollectionLazy.False:
						return CollectionLazy.NoLazy;
					case HbmCollectionLazy.Extra:
						return CollectionLazy.Extra;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			set
			{
				mapping.lazySpecified = true;
				switch (value)
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
		}

		public void Sort() { }

		#endregion
	}
}