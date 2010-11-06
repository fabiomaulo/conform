using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfOrm
{
	public class PolymorphismResolver : IPolymorphismResolver
	{
		private ICollection<Type> domain = new HashSet<Type>();
		private ICollection<Type> exclusions = new HashSet<Type>();
		public IEnumerable<Type> GetBaseImplementors(Type ancestor)
		{
			var result = new HashSet<Type>();
			foreach (var type in domain)
			{
				var implementor = type.GetFirstImplementorOf(ancestor);
				if(implementor != null)
				{
					result.Add(implementor);
				}
			}
			return result.Where(t=> !exclusions.Contains(t));
		}

		public void Add(Type type)
		{
			if (type == null)
			{
				return;
			}
			domain.Add(type);
		}

		public void Exclude(Type type)
		{
			if (type == null)
			{
				return;
			}
			exclusions.Add(type);
		}
	}
}