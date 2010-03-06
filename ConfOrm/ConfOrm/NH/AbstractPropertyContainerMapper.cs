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

		public virtual void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			if (!property.DeclaringType.IsAssignableFrom(container))
			{
				throw new ArgumentOutOfRangeException("property","Can't add a property of another graph");
			}
			var hbmProperty = new HbmProperty { name = property.Name };
			mapping(new PropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public virtual void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			var hbm = new HbmComponent { name = property.Name };
			mapping(new ComponentMapper(hbm, property.GetPropertyOrFieldType(), MapDoc));
			AddProperty(hbm);
		}

		public virtual void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(property, hbm));
			AddProperty(hbm);
		}

		public virtual void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			var hbm = new HbmOneToOne { name = property.Name };
			mapping(new OneToOneMapper(property, hbm));
			AddProperty(hbm);
		}

		public virtual void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmSet { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new SetMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmBag { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new BagMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmList { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			var collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new ListMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public virtual void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmMap { name = property.Name };
			var propertyType = property.GetPropertyOrFieldType();
			Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			Type dictionaryValueType = propertyType.DetermineDictionaryValueType();

			collectionMapping(new MapMapper(container, dictionaryKeyType, dictionaryValueType, hbm, mapDoc));
			mapping(new CollectionElementRelation(dictionaryValueType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		#endregion

		protected Type GetMemberType(MemberInfo propertyOrField)
		{
			return propertyOrField.GetPropertyOrFieldType();
		}
	}
}