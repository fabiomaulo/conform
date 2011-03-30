using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public delegate void RootClassMappingHandler(IDomainInspector domainInspector, Type type, IClassAttributesMapper classCustomizer);
	public delegate void SubclassMappingHandler(IDomainInspector domainInspector, Type type, ISubclassAttributesMapper subclassCustomizer);
	public delegate void JoinedSubclassMappingHandler(IDomainInspector domainInspector, Type type, IJoinedSubclassAttributesMapper joinedSubclassCustomizer);
	public delegate void UnionSubclassMappingHandler(IDomainInspector domainInspector, Type type, IUnionSubclassAttributesMapper unionSubclassCustomizer);

	public delegate void PropertyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IPropertyMapper propertyCustomizer);
	public delegate void ManyToOneMappingHandler(IDomainInspector domainInspector, PropertyPath member, IManyToOneMapper propertyCustomizer);
	public delegate void OneToOneMappingHandler(IDomainInspector domainInspector, PropertyPath member, IOneToOneMapper propertyCustomizer);
	public delegate void AnyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IAnyMapper propertyCustomizer);
	public delegate void ComponentMappingHandler(IDomainInspector domainInspector, PropertyPath member, IComponentAttributesMapper propertyCustomizer);

	public delegate void SetMappingHandler(IDomainInspector domainInspector, PropertyPath member, ISetPropertiesMapper propertyCustomizer);
	public delegate void BagMappingHandler(IDomainInspector domainInspector, PropertyPath member, IBagPropertiesMapper propertyCustomizer);
	public delegate void ListMappingHandler(IDomainInspector domainInspector, PropertyPath member, IListPropertiesMapper propertyCustomizer);
	public delegate void MapMappingHandler(IDomainInspector domainInspector, PropertyPath member, IMapPropertiesMapper propertyCustomizer);

	public delegate void ManyToManyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IManyToManyMapper collectionRelationManyToManyCustomizer);
	public delegate void ElementMappingHandler(IDomainInspector domainInspector, PropertyPath member, IElementMapper collectionRelationElementCustomizer);
	public delegate void OneToManyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IOneToManyMapper collectionRelationOneToManyCustomizer);

	public delegate void MapKeyManyToManyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IMapKeyManyToManyMapper mapKeyManyToManyCustomizer);
	public delegate void MapKeyMappingHandler(IDomainInspector domainInspector, PropertyPath member, IMapKeyMapper mapKeyElementCustomizer);
}