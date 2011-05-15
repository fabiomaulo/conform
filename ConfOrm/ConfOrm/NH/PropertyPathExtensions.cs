using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.NH
{
	public static class PropertyPathExtensions
	{
		public static Type GetContainerEntity(this PropertyPath propertyPath, IDomainInspector domainInspector)
		{
			PropertyPath analizing = propertyPath;
			while (analizing.PreviousPath != null && !domainInspector.IsEntity(analizing.LocalMember.ReflectedType))
			{
				analizing = analizing.PreviousPath;
			}
			return analizing.LocalMember.ReflectedType;
		}
	}
}