using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;

namespace ConfOrm
{
	public static class TypeExtensions
	{
		public static IEnumerable<Type> GetBaseTypes(this Type type)
		{
			var analizing = type;
			while (analizing != null && analizing != typeof(object))
			{
				analizing = analizing.BaseType;
				yield return analizing;
			}
		}

		public static string GetTypeName(this Type typeElement)
		{
			string typeName;
			var nhType = TypeFactory.HeuristicType(typeElement.AssemblyQualifiedName);
			if (nhType != null)
			{
				typeName = nhType.Name;
			}
			else
			{
				typeName = typeElement.FullName + ", " + typeElement.Assembly.GetName().Name;
			}
			return typeName;
		}

		public static string GetClassName(this Type type, HbmMapping mapDoc)
		{
			var typeAssembly = type.Assembly.GetName().Name;
			var typeNameSpace = type.Namespace;
			string assembly = null;
			if (!typeAssembly.Equals(mapDoc.assembly))
			{
				assembly = typeAssembly;
			}
			string @namespace = null;
			if (!typeNameSpace.Equals(mapDoc.@namespace))
			{
				@namespace = typeNameSpace;
			}
			if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.AssemblyQualifiedName;
			}
			if (!string.IsNullOrEmpty(assembly) && string.IsNullOrEmpty(@namespace))
			{
				return string.Concat(type.Name, ", ", assembly);
			}
			if (string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(@namespace))
			{
				return type.FullName;
			}

			return type.Name;
		}
	}
}