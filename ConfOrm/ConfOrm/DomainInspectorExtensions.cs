using System;
using System.Linq;

namespace ConfOrm
{
	public static class DomainInspectorExtensions
	{
		public static Type GetRootEntity(this IDomainInspector domainInspector, Type entityType)
		{
			if (entityType == null)
			{
				return null;
			}
			if(domainInspector.IsRootEntity(entityType))
			{
				return entityType;
			}
			return entityType.GetBaseTypes().SingleOrDefault(domainInspector.IsRootEntity);
		}
	}
}