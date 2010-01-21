using System;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public abstract class AbstractPropertyContainerMapper : IPropertyContainerMapper
	{
		private readonly Type container;
		private readonly HbmMapping mapDoc;

		protected AbstractPropertyContainerMapper(Type container, HbmMapping mapDoc)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			if (mapDoc == null)
			{
				throw new ArgumentNullException("mapDoc");
			}
			this.container = container;
			this.mapDoc = mapDoc;
		}

		protected HbmMapping MapDoc
		{
			get { return mapDoc; }
		}

		protected Type Container
		{
			get { return container; }
		}

		protected abstract void AddProperty(object property);

		#region Implementation of IPropertyContainerMapper

		public void Property(MemberInfo property)
		{
			if (!property.DeclaringType.IsAssignableFrom(container))
			{
				throw new ArgumentOutOfRangeException("property","Can't add a property of another graph");
			}
			var hbmProperty = new HbmProperty { name = property.Name };
			AddProperty(hbmProperty);
		}

		public void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void ManyToOne(MemberInfo property)
		{
			throw new NotImplementedException();
		}

		public void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Set(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void Bag<TElement>(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void List<TElement>(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void Map<TKey, TElement>(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}