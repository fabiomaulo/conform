using System;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public delegate	void RootClassMappingHandler(IDomainInspector domainInspector, Type type, IClassAttributesMapper classCustomizer);
}