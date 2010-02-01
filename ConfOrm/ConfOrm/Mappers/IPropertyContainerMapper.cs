using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IPropertyContainerMapper
	{
		void Property(MemberInfo property);

		void Component(MemberInfo property, Action<IComponentMapper> mapping);

		void ManyToOne(MemberInfo property);
		void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping);

		void Set(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		         Action<ICollectionElementRelation> mapping);

		void Bag(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                   Action<ICollectionElementRelation> mapping);

		void List(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                    Action<ICollectionElementRelation> mapping);

		void Map<TKey, TElement>(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                         Action<ICollectionElementRelation> mapping);
	}
}