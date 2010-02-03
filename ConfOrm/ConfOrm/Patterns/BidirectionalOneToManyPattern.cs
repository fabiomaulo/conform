using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyPattern : IPattern<Relation>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		#region Implementation of IPattern<Relation>

		public bool Match(Relation subject)
		{
			// the 'from' represent the one-side, the 'to' represent the many-side
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			Type one = subject.From;
			Type many = subject.To;

			return HasCollectionOf(one, many) && HasPropertyOf(many, one);
		}

		#endregion

		private bool HasCollectionOf(Type one, Type many)
		{
			foreach (Type propertyType in
				one.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType))
			{
				if (propertyType.IsGenericCollection())
				{
					List<Type> interfaces = propertyType.GetInterfaces().Where(t => t.IsGenericType).ToList();
					if (propertyType.IsInterface)
					{
						interfaces.Add(propertyType);
					}
					Type genericEnumerable = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
					if (genericEnumerable != null)
					{
						Type genericArgument = genericEnumerable.GetGenericArguments()[0];
						if (genericArgument.IsAssignableFrom(many))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool HasPropertyOf(Type many, Type one)
		{
			return many.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType).Any(t => t.IsAssignableFrom(one));
		}
	}
}