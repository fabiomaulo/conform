using System;
using ConfOrm.Patterns;

namespace ConfOrm
{
	public class DefaultPatternsHolder: PatternsHolder
	{
		public DefaultPatternsHolder(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			Poids.Add(new PoIdPattern());
			Sets.Add(new SetCollectionPattern());
			Bags.Add(new BagCollectionPattern());
			Lists.Add(new ListCollectionPattern(domainInspector));
			Arrays.Add(new ArrayCollectionPattern());
			Componets.Add(new ComponentPattern(domainInspector));
			Dictionaries.Add(new DictionaryCollectionPattern());
			Cascades.Add(new BidirectionalOneToManyCascadePattern(domainInspector));

			PoidStrategies.Add(new HighLowPoidPattern());
			PoidStrategies.Add(new GuidOptimizedPoidPattern());

			PersistentPropertiesExclusions.Add(new ReadOnlyPropertyPattern());
		}
	}
}