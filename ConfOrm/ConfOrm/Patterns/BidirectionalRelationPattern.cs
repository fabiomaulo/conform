using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalRelationPattern: IPattern<Relation>
	{
		#region Implementation of IPattern<Relation>

		public bool Match(Relation subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var fromHasRelationWithTo = HasRelation(subject.From, subject.To);
			var toHasRelationWithFrom = HasRelation(subject.To, subject.From);
			return fromHasRelationWithTo && toHasRelationWithFrom;
		}

		private bool HasRelation(Type from, Type to)
		{
			foreach (var propertyType in from.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Select(p => p.PropertyType))
			{
				if(propertyType.IsAssignableFrom(to))
				{
					return true;
				}
				if (propertyType.IsGenericCollection())
				{
					List<Type> interfaces =
						propertyType.GetInterfaces().Where(t => t.IsGenericType).ToList();
					if (propertyType.IsInterface)
					{
						interfaces.Add(propertyType);
					}
					var genericEnumerable = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
					if(genericEnumerable != null)
					{
						var genericArgument = genericEnumerable.GetGenericArguments()[0];
						if (genericArgument.IsAssignableFrom(to))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		#endregion
	}
}