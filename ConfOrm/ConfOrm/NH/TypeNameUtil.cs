using System;
using NHibernate.Type;

namespace ConfOrm.NH
{
	public static class TypeNameUtil
	{
		public static string GetNhTypeName(this Type type)
		{
			string typeName;
			var nhType = TypeFactory.HeuristicType(type.AssemblyQualifiedName);
			if (nhType != null)
			{
				typeName = nhType.Name;
			}
			else
			{
				typeName = type.FullName + ", " + type.Assembly.GetName().Name;
			}
			return typeName;
		}
	}
}