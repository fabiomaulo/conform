using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfOrm
{
	public class PolymorphismResolver : IPolymorphismResolver
	{
		private HashSet<Type> domain = new HashSet<Type>();
		private HashSet<Type> exclusions = new HashSet<Type>();
		private readonly Dictionary<Type, IEnumerable<Type>> knownSolution = new Dictionary<Type, IEnumerable<Type>>();

		public IEnumerable<Type> GetBaseImplementors(Type ancestor)
		{
			if (ancestor == null)
			{
				return Enumerable.Empty<Type>();
			}
			IEnumerable<Type> result;
			if(!knownSolution.TryGetValue(ancestor, out result))
			{
				var partialResult = new HashSet<Type>();
				foreach (var type in domain)
				{
					var implementor = type.GetFirstImplementorOf(ancestor);
					if (implementor != null)
					{
						partialResult.Add(implementor);
					}
				}
				result = partialResult.Where(t => !exclusions.Contains(t)).ToArray();
				knownSolution[ancestor] = result;
			}
			return result;
		}

		public void Add(Type type)
		{
			if (type == null)
			{
				return;
			}
			
			if(domain.Add(type))
			{
				InvalidateKnownSolution();
			}
		}

		private void InvalidateKnownSolution()
		{
			knownSolution.Clear();
		}

		public void Exclude(Type type)
		{
			if (type == null)
			{
				return;
			}
			if (exclusions.Add(type))
			{
				InvalidateKnownSolution();
			}
		}
	}
}