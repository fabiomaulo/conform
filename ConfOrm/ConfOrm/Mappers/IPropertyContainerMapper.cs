using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IPropertyContainerMapper
	{
		void Property(MemberInfo property, Action<IPropertyMapper> mapping);

		void Component(MemberInfo property, Action<IComponentMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping);
		void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping);

		void Set(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		         Action<ICollectionElementRelation> mapping);

		void Bag(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                   Action<ICollectionElementRelation> mapping);

		void List(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                    Action<ICollectionElementRelation> mapping);

		void Map(MemberInfo property, Action<ICollectionPropertiesMapper> collectionMapping,
		                         Action<ICollectionElementRelation> mapping);
	}
}