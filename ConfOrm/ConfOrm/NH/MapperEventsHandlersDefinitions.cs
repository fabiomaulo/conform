using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public delegate void RootClassMappingHandler(IDomainInspector domainInspector, Type type, IClassAttributesMapper classCustomizer);
	public delegate void SubclassMappingHandler(IDomainInspector domainInspector, Type type, ISubclassAttributesMapper subclassCustomizer);
	public delegate void JoinedSubclassMappingHandler(IDomainInspector domainInspector, Type type, IJoinedSubclassAttributesMapper joinedSubclassCustomizer);

}