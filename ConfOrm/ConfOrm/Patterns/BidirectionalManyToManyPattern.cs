using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyPattern: IPattern<MemberInfo>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var propertyType = subject.GetPropertyOrFieldType();
			if (!propertyType.IsGenericCollection())
			{
				// can't determine relation for a no generic collection
				return false;
			}

			var fromMany = subject.DeclaringType;
			Type cadidateToMany = propertyType.DetermineCollectionElementType();
			if (cadidateToMany.IsGenericType && typeof(KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
			{
				// many-to-many on map
				var dictionaryGenericArguments = cadidateToMany.GetGenericArguments();
				return HasCollectionOf(dictionaryGenericArguments[1], fromMany) || HasCollectionOf(dictionaryGenericArguments[0], fromMany);
			}
			else
			{
				// many-to-many on plain collection
				var toMany = cadidateToMany;
				return HasCollectionOf(toMany, fromMany);
			}
		}

		#endregion

		protected bool HasCollectionOf(Type fromMany, Type toMany)
		{
			foreach (Type propertyType in
				fromMany.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType))
			{
				if (!propertyType.IsGenericCollection())
				{
					continue;
				}
				List<Type> interfaces = propertyType.GetInterfaces().Where(t => t.IsGenericType).ToList();
				if (propertyType.IsInterface)
				{
					interfaces.Add(propertyType);
				}
				Type genericEnumerable = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (genericEnumerable != null)
				{
					Type genericArgument = genericEnumerable.GetGenericArguments()[0];
					if (genericArgument.IsAssignableFrom(toMany))
					{
						return true;
					}
					if (genericArgument.IsGenericType && typeof(KeyValuePair<,>) == genericArgument.GetGenericTypeDefinition())
					{
						var dictionaryGenericArguments = genericArgument.GetGenericArguments();
						if (dictionaryGenericArguments[1].IsAssignableFrom(toMany) || dictionaryGenericArguments[0].IsAssignableFrom(toMany))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}