using System;
using System.Collections.Generic;

namespace ConfOrmTests.PolymorphismRelationsTests
{
	public class DomainAnalyzer
	{
		private ICollection<Type> domain = new HashSet<Type>();
		public IEnumerable<Type> GetBaseImplementors(Type ancestor)
		{
			foreach (var type in domain)
			{
				if(ancestor.IsAssignableFrom(type))
				{
					yield return type;
				}
			}
		}

		public void Add(Type type)
		{
			if (type == null)
			{
				return;
			}
			domain.Add(type);
		}
	}
}